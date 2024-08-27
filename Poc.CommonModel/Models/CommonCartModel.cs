using System.ComponentModel.DataAnnotations;

namespace POC.CommonModel.Models
{
    public class CommonCartModel
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;

    }
}
