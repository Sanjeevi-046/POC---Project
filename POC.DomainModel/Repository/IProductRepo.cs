using Poc.CommonModel.Models;
using POC.CommonModel.Models;

namespace POC.DomainModel.Repository
{
    public interface IProductRepo
    {

        Task<CommonPaginationModel> GetAllProducts(int page, string searchName = "");
        Task<CommonProductModel> GetProductById(int id);
        Task<UserValidationResult> AddProduct(CommonProductModel product);
        Task<MemoryStream> DownloadExcelFile();
    }
}
