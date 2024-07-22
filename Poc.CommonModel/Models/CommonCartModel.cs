using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace POC.MVC.Models
{
    public class CartModel
    {
        [Key]
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }

    }
}
