using Poc.CommonModel.Models;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
