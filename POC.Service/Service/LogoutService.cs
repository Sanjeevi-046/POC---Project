using POC.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public class LogoutService:ILogout
    {
        private readonly ILogoutRepo logoutRepo;
        public LogoutService(ILogoutRepo logoutRepo)
        {
            this.logoutRepo = logoutRepo;
        }
        public async Task<bool> Logout(int id)
        {
            var Logout = await logoutRepo.LogoutAsync(id);
            return Logout;
        }
    }
}
