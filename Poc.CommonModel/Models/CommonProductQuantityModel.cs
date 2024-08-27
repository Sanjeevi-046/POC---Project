using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.CommonModel.Models
{
    public class CommonProductQuantityModel
    {
        public CommonProductModel? ProductList { get; set; }
        public int Quantity { get; set; } = 1;
        public int UserID { get; set; }
        public int ProductID { get; set; }

    }
}
