using System.ComponentModel.DataAnnotations;

namespace PaymentModels
{
    public class CreditCardModel
    {
        const string creditCardRegex = "^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\\d{3})\\d{11})$";

        [Required(ErrorMessage = "Credit card number can not be null!")]
        [RegularExpression(creditCardRegex)]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration Month can not be null!")]
        [Range(1, 12)]
        public int ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Expiration Year can not be null!")]
        [Range(2020, 2050)]
        public int ExpirationYear { get; set; }

        [Required(ErrorMessage = "Credit card Cvv number can not be null!")]
        public string Cvv { get; set; }
    }
}
