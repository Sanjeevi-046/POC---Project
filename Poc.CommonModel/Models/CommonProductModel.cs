using System.ComponentModel.DataAnnotations;

namespace POC.CommonModel.Models
{

    public class CommonProductModel
    {
        public int ProductId { get; set; }
        [RegularExpression(@"[a-zA-Z0-9 ]+", ErrorMessage = "Name cannot contain special characters"), Required]
        public string? Name { get; set; }
        [Required]
        [Range(100, double.MaxValue, ErrorMessage = "Price must be above 100")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public byte[] ProductImage { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsQuantityAvailable { get; set; }
        [Range(1, 10, ErrorMessage = "ProductAvailable must be below 10")]
        public int ProductAvailable { get; set; }

    }
}
