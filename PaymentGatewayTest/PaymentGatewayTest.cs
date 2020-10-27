using PaymentGatewayAPI;
using PaymentModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using PaymentGatewayAPI.DataAccess.Entities;

namespace PaymentGatewayTest
{
    public class PaymentGatewayTest
    {
        private readonly HttpClient _client;

        public PaymentGatewayTest()
        {
            var appFactory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Startup>();
            _client = appFactory.CreateClient();
        }

        #region Post Tests

        [Fact]
        public async Task PostPaymentGetTrackingInfoForMock1Return200()
        {
            AddApiKeyForMockTenant1();
            Guid trackingNumberForMock1 = await DoValidPostOperation();

            var getResult = await _client.GetAsync($"payment?trackingNumber={trackingNumberForMock1}");
            Assert.True(getResult.IsSuccessStatusCode);
            Assert.True(getResult.StatusCode == System.Net.HttpStatusCode.OK);

            var getResultContent = await getResult.Content?.ReadAsStringAsync();

            var deserializedGetResultObject = JsonConvert.DeserializeObject<ResponseModel<ResponseLogEntity>>(getResultContent);
            Assert.NotNull(deserializedGetResultObject?.Data?.CardNumber);
        }

        [Fact]
        public async Task PostPaymentForMock1GetTrackingInfoForMock2Return404()
        {
            AddApiKeyForMockTenant1();
            Guid trackingNumberForMock1 = await DoValidPostOperation();

            AddApiKeyForMockTenant2();
            var getResult = await _client.GetAsync($"payment?trackingNumber={trackingNumberForMock1}");
            Assert.False(getResult.IsSuccessStatusCode);
            Assert.True(getResult.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostPaymentForMock2Return502()
        {
            AddApiKeyForMockTenant2();
            var paymentModel = GetCompletePaymentModel();

            var result = await _client.PostAsJsonAsync("payment", paymentModel);
            Assert.False(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadGateway);

            var resultData = await result.Content?.ReadAsStringAsync();
            var deserializedPostResultObject = JsonConvert.DeserializeObject<ResponseModel<Guid>>(resultData);
            Assert.NotNull(deserializedPostResultObject);
            Assert.False(deserializedPostResultObject.Success);
        }

        [Fact]
        public async Task PostWithoutApiKeyReturn401()
        {
            _client.DefaultRequestHeaders.Clear();
            var result = await _client.PostAsJsonAsync("payment", GetCompletePaymentModel());
            Assert.False(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostInvalidIPReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.PaymentSourceIp = "thisisaninvalidip";
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostInvalidCurrencyReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.Currency = "XXX";
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostEmptyDataReturn400()
        {
            AddApiKeyForMockTenant1();
            await CheckBadRequest(new PaymentModel());
        }

        [Fact]
        public async Task PostZeroAmountReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.Amount = 0;
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostInvalidCardNumberReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.CreditCartInfo.CardNumber = "thisisaninvalidcard";
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostInvalidMonthInfoReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.CreditCartInfo.ExpirationMonth = 99;
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostInvalidYearReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.CreditCartInfo.ExpirationYear = 999999;
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostInvalidCvvReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.CreditCartInfo.Cvv = "0";
            await CheckBadRequest(data);
        }

        [Fact]
        public async Task PostNullCardInfoReturn400()
        {
            AddApiKeyForMockTenant1();
            var data = GetCompletePaymentModel();
            data.CreditCartInfo = null;
            await CheckBadRequest(data);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task GetWithoutApiKeyReturn401()
        {
            _client.DefaultRequestHeaders.Clear();
            var result = await _client.GetAsync($"payment?trackingNumber={Guid.NewGuid()}");
            Assert.False(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetWithInvalidApiKeyReturn401()
        {
            AddApiKeyToHeader(Guid.NewGuid().ToString());
            var result = await _client.GetAsync($"payment?trackingNumber={Guid.NewGuid()}");
            Assert.False(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetWithNewGuidReturn404()
        {
            AddApiKeyForMockTenant1();
            var getResult = await _client.GetAsync($"payment?trackingNumber={Guid.NewGuid()}");
            Assert.False(getResult.IsSuccessStatusCode);
            Assert.True(getResult.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetWithStringReturn400()
        {
            AddApiKeyForMockTenant1();
            var getResult = await _client.GetAsync($"payment?trackingNumber=thisisnotguid");
            Assert.False(getResult.IsSuccessStatusCode);
            Assert.True(getResult.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        #endregion

        private async Task CheckBadRequest(PaymentModel data)
        {
            var result = await _client.PostAsJsonAsync("payment", data);
            Assert.False(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        private void AddApiKeyForMockTenant1() => AddApiKeyToHeader("034315C9-6BDE-41A2-AAE1-0A241FA8F5CE");

        private void AddApiKeyForMockTenant2() => AddApiKeyToHeader("234FA67C-598F-4FBE-BE5F-D855DC192F22");

        private void AddApiKeyToHeader(string tenantGuid)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("ApiKey", tenantGuid);
        }

        private PaymentModel GetCompletePaymentModel() => new PaymentModel()
        {
            Amount = 10,
            Currency = "EUR",
            PaymentDescription = "Payment test for mock tenant 1",
            PaymentSourceIp = "192.168.1.65",
            SourceOfPayment = "Amazon.com",
            CreditCartInfo = new CreditCardModel()
            {
                CardNumber = "4012888888881881",
                ExpirationMonth = 8,
                ExpirationYear = 2021,
                Cvv = "123",
            }
        };

        private async Task<Guid> DoValidPostOperation()
        {
            var paymentModel = GetCompletePaymentModel();

            var result = await _client.PostAsJsonAsync("payment", paymentModel);
            Assert.True(result.IsSuccessStatusCode);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Created);

            var resultData = await result.Content?.ReadAsStringAsync();
            var deserializedPostResultObject = JsonConvert.DeserializeObject<ResponseModel<Guid>>(resultData);
            Assert.NotNull(deserializedPostResultObject);
            Assert.True(deserializedPostResultObject.Success);

            return deserializedPostResultObject.TrackingNumber;
        }
    }
}
