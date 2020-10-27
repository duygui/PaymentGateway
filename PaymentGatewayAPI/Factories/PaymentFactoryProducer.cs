using PaymentModels;

namespace PaymentGatewayAPI
{
    public class PaymentFactoryProducer
    {
        private readonly IPaymentFactory _creditCardFactory;
        private readonly IPaymentFactory _apmFactory;

        public PaymentFactoryProducer(IPaymentFactory creditCardFactory, IPaymentFactory apmFactory)
        {
            _creditCardFactory = creditCardFactory;
            _apmFactory = apmFactory;
        }
      
        public IPaymentFactory GetPaymentOptionFactory(EPaymentOption paymentOption)
        {
            return paymentOption == EPaymentOption.CreditCard
                ? _creditCardFactory
                : _apmFactory;
        }
    }
}
