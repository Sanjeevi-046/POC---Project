using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POC.CommonModel.Models;
using POC.DataLayer.Models;

namespace POC.DataLayer.Repository
{
    public class AddressRepository : IAddressRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper mapper;

        public AddressRepository(DemoProjectContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        //Get: Address List
        public async Task<List<CommonAddressModel>> GetAddress(int userId)
        {
            var addresses = await _context.AddressDetails.Where(x => x.UserId == userId).ToListAsync();
            return mapper.Map<List<CommonAddressModel>>(addresses);
        }
        //Get : SIngle Address
        public async Task<CommonAddressModel> GetAddressID(int AddressId)
        {
            var address = await _context.AddressDetails.Where(x => x.Id == AddressId).SingleOrDefaultAsync();

            return mapper.Map<CommonAddressModel>(address);
        }
        public async Task<bool> AddAddress(CommonAddressModel commonAddressModel)
        {
            var Address = mapper.Map<AddressDetail>(commonAddressModel);
            try
            {
                await _context.AddressDetails.AddAsync(Address);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditAddress(CommonAddressModel commonAddressModel)
        {
            var address = mapper.Map<AddressDetail>(commonAddressModel);
            var existingAddress = await _context.AddressDetails.FindAsync(address.Id);
            if (existingAddress == null)
            {
                return false;
            }

            _context.Entry(existingAddress).CurrentValues.SetValues(address);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SetDefaultAsync(int AddressId)
        {
            var address = await _context.AddressDetails.FirstOrDefaultAsync(a => a.Id == AddressId);
            if (address == null)
            {
                return false;
            }
            await _context.AddressDetails.Where(a => a.UserId == address.UserId).ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDefault, false));
            //var userAddresses = await _context.AddressDetails.Where(a => a.UserId == address.UserId).ToListAsync();
            //userAddresses.ForEach(a => a.IsDefault = false);
            address.IsDefault = true;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAddress(int id)
        {
            var address = await _context.AddressDetails.FindAsync(id);
            if (address == null)
            {
                return false;
            }

            _context.AddressDetails.Remove(address);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
