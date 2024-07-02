using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using POC.DomainModel.Models;

namespace POC.MVC.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int? Id { get; set; }

        public decimal OrderPrice { get; set; }

        public virtual Login ?IdNavigation { get; set; }

        public virtual Product ?Product { get; set; }



    }
}
