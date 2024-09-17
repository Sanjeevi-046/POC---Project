using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public interface IAdmin
    {
        Task<CommonPaginationModel> GetOrderAsync(string sortColumnName = "Order ID", string sortOrder = "asc", string Status = "", string SearchName = "", DateTime? fromDate = null, DateTime? toDate = null,int Page= 1);
        Task<MemoryStream> GetInvoicePdfAsync(int orderID);
        Task<MemoryStream> DownloadExcel();

    }
}
