using AutoMapper;
using POC.CommonModel.Models;
using POC.DataLayer.Models;

namespace POC.DataLayer.Repository
{
    public class UserRepository:IUserRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DemoProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommonLoginModel> GetUserAsync(int id)
        {
            var details = await _context.Logins.FindAsync(id);
            return _mapper.Map<CommonLoginModel>(details);
        }
        public async Task<bool> UpdateUserAsync(CommonUserModel userModel)
        {
            var details = await _context.Logins.FindAsync(userModel.Id);
            if (details != null) 
            { 
                details.Surname = userModel.Surname;
                details.Email = userModel.Email;
                details.Address = userModel.Address;
                details.MobileNumber = userModel.MobileNumber;
                details.Country = userModel.Country;
                details.State = userModel.State;
                details.Postcode = userModel.Postcode;
                _context.Logins.Update(details);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
