using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public class AdminService:IAdmin
    {
        private readonly IAdminRepo _adminRepo;
        public AdminService(IAdminRepo adminRepo) { _adminRepo = adminRepo; }
        public async Task<CommonPaginationModel> GetOrderAsync(string sortColumnName = "Order ID", string sortOrder = "asc", string Status = "", string SearchName = "", DateTime? fromDate = null, DateTime? toDate = null, int Page = 1)
        {
            return await _adminRepo.GetOrderAsync(sortColumnName, sortOrder, Status, SearchName , fromDate,toDate,Page);
        }

        public async Task<MemoryStream> GetInvoicePdfAsync(int orderID)
        {
            return await _adminRepo.GetInvoicePdfAsync(orderID);
        }

        public async Task<MemoryStream> DownloadExcel()
        {
            return await _adminRepo.DownloadExcel();
        }


    }
}
