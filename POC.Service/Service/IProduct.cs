using Poc.CommonModel.Models;
using POC.CommonModel.Models;

namespace POC.ServiceLayer.Service
{
    public interface IProduct
    {
        Task<CommonPaginationModel> GetAllProductsAsync(int page, string searchName = "");
        Task<CommonProductModel> GetProductByIdAsync(int id);
        Task<UserValidationResult> AddProductAsync(CommonProductModel product);
        Task<MemoryStream> DownloadExcel();
        Task<MemoryStream> DownloadHtml();
    }
}
