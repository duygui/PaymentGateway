using Microsoft.Extensions.Configuration;
using PaymentModels;
using PaymentGatewayAPI.Services.CreditCardServices;
using System;
using PaymentGatewayAPI.DataAccess;
using PaymentModels.Enumeration;
using PaymentGatewayAPI.Helpers;

namespace PaymentGatewayAPI
{
    public class CreditCardPaymentFactory : IPaymentFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IDataOperations _dataOperations;

        public CreditCardPaymentFactory(IConfiguration configuration, IDataOperations dataOperations)
        {
            _configuration = configuration;
            _dataOperations = dataOperations;
        }

        public CreditCardPaymentService GetPaymentService()
        {
            var acquiringBank = _dataOperations.GetAcquiringBank();
            return acquiringBank switch
            {
                EAcquiringBanks.HSBCBank => new HSBCService(_configuration),
                EAcquiringBanks.INGBank => new INGService(_configuration),
                _ => throw new Exception("Acquiring bank info not found!"),
            };
        }

        public ResponseModel<Guid> ValidateModelForPaymentOption(PaymentModel model, Guid trackingNumber)
        {
            if (model.CreditCartInfo == null)
                return ResponseHelper.GetFailureResponse(trackingNumber, Guid.Empty, System.Net.HttpStatusCode.BadRequest, "Please enter card info.");

            if (!Enum.TryParse(model.Currency, out ECurrencyCodes eCurrencyCode))
                return ResponseHelper.GetFailureResponse(trackingNumber, Guid.Empty, System.Net.HttpStatusCode.BadRequest, "Please enter a valid ISO Currency code.");

            if (string.IsNullOrEmpty(model.CreditCartInfo.Cvv) || !int.TryParse(model.CreditCartInfo.Cvv, out int cvv) || cvv < 100 || cvv > 999)
                return ResponseHelper.GetFailureResponse(trackingNumber, Guid.Empty, System.Net.HttpStatusCode.BadRequest, "Please enter a valid cvv value.");
            model.CreditCartInfo.Cvv = model.CreditCartInfo.Cvv.Encrypt();
            return ResponseHelper.GetSuccessResponse(trackingNumber, Guid.Empty);
        }
    }
}
