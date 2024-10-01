using System.ComponentModel.DataAnnotations;

namespace Final.Mvc.ViewModels.UserVMs
{

    public class PaymentVM
    {
        [Required(ErrorMessage = "Cardholder name is required.")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "Card number is required.")]
        [CreditCard(ErrorMessage = "Invalid card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        [RegularExpression(@"\d{2}/\d{2}", ErrorMessage = "Expiration date must be in MM/YY format.")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"\d{3}", ErrorMessage = "CVV must be 3 digits.")]
        public string CVV { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }


}
