using System.ComponentModel.DataAnnotations;

namespace POC.MVC.Models
{
    public class LoginModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

    }
}
