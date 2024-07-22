using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Poc.CommonModel.Models
{
    public class CommonRefereshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("expiration_time", TypeName = "datetime")]
        public DateTime ExpirationTime { get; set; }

        [Column("userId")]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string RefreshToken1 { get; set; }

    }
}


   