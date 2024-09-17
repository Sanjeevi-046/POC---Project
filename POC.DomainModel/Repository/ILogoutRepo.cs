using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataLayer.Repository
{
    public interface ILogoutRepo
    {
        Task<bool> LogoutAsync(int id);
    }
}
