using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public interface IAddress
    {
        Task<List<CommonAddressModel>> GetAddress(int UserId);
        Task<CommonAddressModel> GetAddressId(int AddressID);
        Task<bool> AddAddress(CommonAddressModel commonAddressModel);
        Task<bool> EditAddress(CommonAddressModel commonAddressModel);
        Task<bool> DeleteAddress(int id);
        Task<bool> SetDefaultAsync(int AddressId);

    }
}
