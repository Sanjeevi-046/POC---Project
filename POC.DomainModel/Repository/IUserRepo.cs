using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataLayer.Repository
{
    public interface IUserRepo
    {
        Task<CommonLoginModel> GetUserAsync(int id);
        Task<bool> UpdateUserAsync(CommonUserModel commonUser);
    }
}
