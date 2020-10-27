﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentModels;
using System.Threading.Tasks;

namespace PaymentGatewayAPI.Services.CreditCardServices
{
    public class HSBCService : CreditCardPaymentService
    {
        private readonly IConfiguration _configuration;

        public HSBCService(IConfiguration configuration) => _configuration = configuration;

        public override string GetServiceAddress() =>_configuration["ServiceAddresses:HsbcServiceUrl"];

        public override string PrepareRequestData(PaymentModel paymentModel)
        {
            //Do Some Mapping Here (if needed) -> according to what external service is expecting
            return JsonConvert.SerializeObject(paymentModel);
        }

        public override Task<ResponseModel> CallDestinationPaymentService(string requestData)
        {
            //Do target specific operations here
            return base.CallDestinationPaymentService(requestData);
        }

    }
}
