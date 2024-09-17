using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.CommonModel.Models
{
    public class CommonAddressModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "FullName must contain only alphabetic characters and spaces.")]
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "MobileNumber must be exactly 10 digits.")]
        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Country must contain only alphabetic characters and spaces.")]
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "State must contain only alphabetic characters and spaces.")]
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City must contain only alphabetic characters and spaces.")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Village must contain only alphabetic characters and spaces.")]
        [Required(ErrorMessage = "Village is required")]
        public string Village { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9\s.,#-\/]+$", ErrorMessage = "Landmark must contain only alphanumeric characters and common symbols.")]
        [Required(ErrorMessage = "Landmark is required")]
        public string Landmark { get; set; }

        [StringLength(6)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        [Required(ErrorMessage = "Pincode is required")]
        public string Pincode { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9\s.,#-\/]+$", ErrorMessage = "AreaStreet must contain alphanumeric characters, spaces, and common symbols.")]
        [Required(ErrorMessage = "AreaStreet is required")]
        public string AreaStreet { get; set; }
        public bool IsDefault { get; set; }
    }
}
