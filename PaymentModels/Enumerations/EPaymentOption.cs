namespace PaymentModels
{
    public enum EPaymentOption
    {
        /// <summary>
        /// Credit Card Payments
        /// </summary>
        CreditCard = 0,

        /// <summary>
        /// Paypal Payments
        /// </summary>
        Paypal = 1,

        /// <summary>
        /// ApplePay Payments
        /// </summary>
        ApplePay = 2,

        /// <summary>
        /// AliPay Payments
        /// </summary>
        AliPay = 3,

        //More options can be added
    }
}
