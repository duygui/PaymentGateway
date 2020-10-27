using Newtonsoft.Json;
using PaymentModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGatewayAPI
{
    public abstract class CreditCardPaymentService
    {
        public abstract string GetServiceAddress();

        public abstract string PrepareRequestData(PaymentModel paymentModel);

        public async virtual Task<ResponseModel> CallDestinationPaymentService(string requestData)
        {
            using var client = new HttpClient();
            var serviceAddress = GetServiceAddress();
            if (string.IsNullOrEmpty(serviceAddress))
                return new ResponseModel() { Success = false, Notification = "Requested Service Address is Empty!" };

            client.BaseAddress = new Uri($"{serviceAddress}/payment");
            var buffer = Encoding.UTF8.GetBytes(requestData);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await client.PostAsync(client.BaseAddress, byteContent);
            if (!result.IsSuccessStatusCode)
                return new ResponseModel() { Success = false, Notification = result.ReasonPhrase };

            var resultContent = result.Content?.ReadAsStringAsync()?.Result;
            return string.IsNullOrEmpty(resultContent)
                ? new ResponseModel() { Success = false, Notification = "Transaction Result is Empty!" }
                : JsonConvert.DeserializeObject<ResponseModel>(resultContent);
        }
    }
}
