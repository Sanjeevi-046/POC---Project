using POC.CommonModel.Models;

namespace Poc.CommonModel.Models
{
    public class CommonPaginationModel
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public List<CommonProductModel> Product { get; set; }
    }
}