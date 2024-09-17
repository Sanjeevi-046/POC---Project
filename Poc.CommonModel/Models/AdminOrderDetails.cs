using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.CommonModel.Models
{
    public class AdminOrderDetails
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderPrice { get; set; }
        public int ProductQuantity { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsCancelled { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Landmark { get; set; }
        public string Village { get; set; }
        public string AresStreet { get; set; }
        public string Pincode { get; set; }
        public bool IsDefault { get; set; }

    }
}
