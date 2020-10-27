using System.ComponentModel.DataAnnotations;

namespace PaymentModels
{
    public class PaymentModel
    {
        private const string ipAddressRegex = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";

        public CreditCardModel CreditCartInfo { get; set; }

        [Required(ErrorMessage = "Amount required!")]
        [Range(0.01, 999999.99)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency required!")]
        [StringLength(3, ErrorMessage = "Currency must be 3 characters!", MinimumLength = 3)]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Source of payment required!")]
        [StringLength(200, ErrorMessage = "Source Of Payment can be max 200 characters!")]
        public string SourceOfPayment { get; set; }

        [StringLength(300, ErrorMessage = "Payment Description can be max 300 characters!")]
        public string PaymentDescription { get; set; }

        [RegularExpression(ipAddressRegex)]
        public string PaymentSourceIp { get; set; }
    }
}
