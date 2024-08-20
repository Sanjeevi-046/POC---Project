using POC.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataLayer.TempModel
{
    public class PaginationModel
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public List<Product> Product { get; set; }
    }
}
