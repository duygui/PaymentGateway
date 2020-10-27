using PaymentModels;
using System;

namespace PaymentGatewayAPI
{
   public interface IPaymentFactory
    {
        ResponseModel<Guid> ValidateModelForPaymentOption(PaymentModel model, Guid trackingNumber);

        CreditCardPaymentService GetPaymentService();
    }
}
