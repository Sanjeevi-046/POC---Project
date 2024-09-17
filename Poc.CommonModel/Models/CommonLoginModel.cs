using System.ComponentModel.DataAnnotations;

namespace POC.CommonModel.Models
{
    public class CommonLoginModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
        [StringLength(100)]
        public string ?Surname { get; set; }

        [StringLength(10)]
        
        public string? MobileNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(6)]
       
        public string? Postcode { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }
    }
}