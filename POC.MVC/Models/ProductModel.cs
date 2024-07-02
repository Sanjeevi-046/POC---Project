using POC.DomainModel.Models;
using System.ComponentModel.DataAnnotations;

namespace POC.MVC.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string ProductImage { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsQuantityAvailable { get; set; }

        public int ProductAvailable { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
