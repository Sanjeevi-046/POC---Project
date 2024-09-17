using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataLayer.Repository
{
    public interface IAddressRepo
    {
        Task<List<CommonAddressModel>> GetAddress(int userId);
        Task<CommonAddressModel> GetAddressID(int AddressId);
        Task<bool> AddAddress(CommonAddressModel commonAddressModel);
        Task<bool> EditAddress(CommonAddressModel commonAddressModel);
        Task<bool> DeleteAddress(int id);
        Task<bool> SetDefaultAsync(int AddressId);
    }
}
