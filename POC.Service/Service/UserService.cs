using AutoMapper;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;

namespace POC.ServiceLayer.Service
{
    public class UserService : IUser
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepo userRepo , IMapper mapper)
        {
           _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<CommonUserModel> GetUser (int id)
        {
            var detail = await _userRepo.GetUserAsync (id);
            return _mapper.Map<CommonUserModel> (detail);
        }
        public async Task<UserValidationResult> UpdateUser(CommonUserModel userModel)
        {
            var detail = await _userRepo.UpdateUserAsync(userModel);
            if (detail)
            {
                return new UserValidationResult {IsValid=true,Message="Updated Successfully!" };
            }
            return new UserValidationResult { IsValid = false, Message ="Error occured !" };
        }

    }
}
