using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("isDelivered")]
        public bool IsDelivered { get; set; }

        [Column("isCancelled")]
        public bool IsCancelled { get; set; }

        public int ProductQuantity { get; set; }

        [Column("AddressID")]
        public int? AddressId { get; set; }

        public string ProductList { get; set; }

        public string ProductQuantityList { get; set; }


    }
}
