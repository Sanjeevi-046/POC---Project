using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POC.CommonModel.Models
{
    // Custom validation attribute for ExpiryYear
    public class ExpiryYearValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is short expiryYear)
            {
                var currentYear = DateTime.Now.Year;

                if (expiryYear >= currentYear)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"Year must be a valid ExpiryYear");
            }

            return new ValidationResult("Invalid Expiry Year.");
        }
    }

    public class CommonPaymentModel
    {
        
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Card Number is required.")]
        [StringLength(16, ErrorMessage = "Card Number must be at most 16 characters long.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card Number must be exactly 16 digits.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry Month is required.")]
        [Range(1, 12, ErrorMessage = "Expiry Month must be between 1 and 12.")]
        public byte ExpiryMonth { get; set; }
       
        [Required(ErrorMessage = "Expiry Year is required.")]
        [ExpiryYearValidation]        // Custom validation for dynamic year validation
        public short ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be exactly 3 digits.")]
        public short Cvv { get; set; }

        public decimal Amount { get; set; }

        public int? UserId { get; set; }

        public int? OrderId { get; set; }
        public int? CartOrderId { get; set; }

        public bool PaymentReceived { get; set; }
    }
}
