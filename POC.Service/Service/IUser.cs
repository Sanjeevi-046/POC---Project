using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public interface IUser
    {
        Task<CommonUserModel> GetUser(int id);
        Task<UserValidationResult> UpdateUser(CommonUserModel userModel);
    }
}
