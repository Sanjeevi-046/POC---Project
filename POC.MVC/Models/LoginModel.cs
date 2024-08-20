using System.ComponentModel.DataAnnotations;

namespace POC.MVC.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
        public string? Email { get; set;}

        public string? Role { get; set; }

    }
}
