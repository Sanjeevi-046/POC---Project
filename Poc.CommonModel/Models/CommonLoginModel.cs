using System.ComponentModel.DataAnnotations;

namespace POC.CommonModel.Models
{
    public class CommonLoginModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

    }
}
