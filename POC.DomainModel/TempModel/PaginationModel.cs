using POC.DataLayer.Models;

namespace POC.DataLayer.TempModel
{
    public class PaginationModel
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public List<Product> Product { get; set; }
    }
}
