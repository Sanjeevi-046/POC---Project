using System.ComponentModel.DataAnnotations;

namespace POC.MVC.Models
{
    public class UserVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        [StringLength(10)]
        [RegularExpression(@"^[A-Za-z\s]{1,10}$", ErrorMessage = "Surname must be up to 10 alphabetic characters.")]
        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MobileNumber must be exactly 10 digits.")]
        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; }

        [StringLength(255)]
        [RegularExpression(@"^[\d\w\s\.,#\-\/]{1,255}$", ErrorMessage = "Address must be up to 255 characters.")]
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [StringLength(6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Postcode must be exactly 6 digits.")]
        [Required(ErrorMessage = "Postcode is required")]
        public string Postcode { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[A-Za-z\s]{1,20}$", ErrorMessage = "State must be up to 20 alphabetic characters.")]
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^[A-Za-z\s]{1,20}$", ErrorMessage = "Country must be up to 20 alphabetic characters.")]
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }
}
