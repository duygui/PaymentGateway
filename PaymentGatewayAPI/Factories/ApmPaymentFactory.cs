using PaymentModels;
using System;
namespace PaymentGatewayAPI
{
    public class ApmPaymentFactory : IPaymentFactory
    {
        public PaymentService GetPaymentService(PaymentModel paymentModel)
        {
            throw new NotImplementedException();
        }

        public ResponseModel ValidateModelForPaymentOption(PaymentModel model)
        {
            throw new NotImplementedException();
        }
    }
}
