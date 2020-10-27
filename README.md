# Payment Gateway Api
This API provides its users to make the payment process from one single point regardless of the payment method. Currently, credit card payments are supported. Alternative payment methods can be easily added to the project.

# How does it work?
(Assumption)Once a user wants to use Payment Gateway API, they have to register as a tenant with its acquiring bank information, and a unique API Key provided to the user.

The user can send the payment information to the system by making an HTTP POST request with its API key. Data is validated and sensitive information is encrypted at this step. Payment Gateway API will send the card info to the acquirer bank and gets the response, returns information to the user with a tracking number. Users can search for payment details with this number later by making HTTP GET requests. Incoming requests and external bank responses are logged to the database.

# Technical Overview
This project is built on .NET Core 3.1, thanks to that it is cross-platform and capable of running on both Microsoft and Linux servers.
PostgreSQL is used as a Database and Fluent Migrator is used for migrating the tables. The ORM of the project is NPOCO and all query operations are made with the help of Linq. 
Integration tests are written with xUnit so that a test host can be created and requests are made to the system. Payment Gateway API has a swagger UI which makes API calls easier. All methods of the system require a registered API key. Sensitive information is encrypted by the Sha512 algorithm. Payment Gateway API has a request limiting feature which assures stability.

Payment Gateway API decides which system to go to and send requests to the related system. While doing this, it uses the Factory pattern, so that it is easy to add new acquiring banks.
The reason it uses the Factory pattern is making it easy to implement alternative payment options in the future by creating a factory of factories, which is Abstract Factory. This approach makes our system open to extension and close to modification.

# Response Codes
## Post method:
**201 ->Created:** Payment is approved

**400 ->Bad Request:** Data sent is invalid

**401 ->Unauthorized:** API key is missing or invalid

**429 ->Too many requests:** Too many requests send for a specific amount of time

**502 ->BadGateway:** Payment is not approved by the acquiring bank

## Get Method: 
**200 ->OK:** Request succeeded and retrieved the tracking info

**400 ->Bad Request:** Data sent is invalid

**401 ->Unauthorized:** API key is missing or invalid

**404 ->Not Found:** No payment detail found with the given tracking number


# Testing the application
## Prerequisites
  - Please change the connection string on the appsettings.json file

  - Please run the mock services while running both Payment Gateway API and its tests.

# Sample Requests 
Add API Key to request header with key "ApiKey"

Api key for mock tenant 1: 034315C9-6BDE-41A2-AAE1-0A241FA8F5CE 

Api key for mock tenant 2: 234FA67C-598F-4FBE-BE5F-D855DC192F22
## (POST)
### Sample Request
{
  "creditCartInfo": {
    "cardNumber": "4012888888881881",
    "expirationMonth": 1,
    "expirationYear": 2021,
    "cvv": "123"
  },
  "amount": 10,
  "currency": "EUR",
  "sourceOfPayment": "Amazon",
  "paymentDescription": "test payment",
  "paymentSourceIp": "192.168.10.34"
}

### Sample Response 1 (201 Created)
{
    //Acquiring bank tracking number
    "data": "fdf1b807-3f48-492b-944f-6e96123e159a",
    "success": true,
    "notification": "OK",
    //Gateway tracking number
    "trackingNumber": "0e6b381c-6891-434c-9838-37662669063f"
}

### Sample Response 2 (502 Bad Gateway)
{
    "data": "e92a3808-61c0-48bf-8390-a5095bb6d324",
    "success": false,
    "notification": "Insufficient Balance",
    "trackingNumber": "927714d4-dccd-41a9-b2e3-81896a908879"
}

## (GET)
https://localhost:44326/payment?trackingNumber=cc28bf34-85be-48ba-9602-0d86533cce0d

### Sample Response 1 (200 OK)
{
    "data": {
        "trackingNumber": "cc28bf34-85be-48ba-9602-0d86533cce0d",
        "externalReference": "af7302cf-88c4-4c37-b369-2f0b74a48eb8",
        "processingDate": "2020-10-26T19:16:11.572152Z",
        "success": true,
        "statusCode": 201,
        "responsePhase": "OK",
        "cardNumber": "4012********1881",
        "amount": 10.00000,
        "tenantId": 1
    },
    "success": true,
    "notification": "OK",
    "trackingNumber": "cc28bf34-85be-48ba-9602-0d86533cce0d"
}
### Sample Response 2 (404 Not Found)
{
    "data": null,
    "success": false,
    "notification": "Payment info not found with the given identifier",
    "trackingNumber": "cc28bf34-85be-48ba-9602-0d86533cce0e"
}

## Areas Could be Improved
- Alternative payment methods can be added.
- Logging can be made to ElasticSearch
- Requests can be handled with a Message Queue (according to the needs)
