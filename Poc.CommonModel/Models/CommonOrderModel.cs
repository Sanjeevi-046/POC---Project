using System.ComponentModel.DataAnnotations;

namespace POC.CommonModel.Models
{
    public class CommonOrderModel
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int? Id { get; set; }

        public decimal OrderPrice { get; set; }


    }
}
