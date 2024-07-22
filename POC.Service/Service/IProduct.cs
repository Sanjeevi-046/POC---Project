using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poc.CommonModel.Models;

namespace POC.DataAccess.Service
{
    public interface IProduct
    {
        Task<CommonPaginationModel> GetAllProductsAsync(int page , string searchName = "");
        Task<CommonProductModel> GetProductByIdAsync(int id);
        Task<UserValidationResult> AddProductAsync(CommonProductModel product);
        Task<MemoryStream> DownloadExcel();
    }
}
