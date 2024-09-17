using POC.CommonModel.Models;
using POC.DataLayer.Repository;

namespace POC.ServiceLayer.Service
{
    public class AddressService:IAddress
    {
        private readonly IAddressRepo _addressRepo;
        public AddressService(IAddressRepo addressRepo)
        {
            _addressRepo = addressRepo;
        }
        public async Task<List<CommonAddressModel>> GetAddress(int UserId)
        {
            return await _addressRepo.GetAddress(UserId);
        }
        public async Task<CommonAddressModel> GetAddressId(int AddressID)
        {

            return await _addressRepo.GetAddressID(AddressID);
        }
        public async Task<bool> AddAddress(CommonAddressModel commonAddressModel)
        {
            return await _addressRepo.AddAddress(commonAddressModel);
        }
        public async Task<bool> EditAddress(CommonAddressModel commonAddressModel)
        {
            return await _addressRepo.EditAddress(commonAddressModel);
        }

        public async Task<bool> DeleteAddress(int id)
        {
            return await _addressRepo.DeleteAddress(id);
        }
        public async Task<bool> SetDefaultAsync(int AddressId)
        {
            return await _addressRepo.SetDefaultAsync(AddressId);
        }
    }
}
