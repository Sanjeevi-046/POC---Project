using Microsoft.EntityFrameworkCore;
using POC.DataLayer.Models;

namespace POC.DataLayer.Repository
{
    public class LogoutRepository :ILogoutRepo //,IDisposable
    {
        private readonly DemoProjectContext context;
        public LogoutRepository(DemoProjectContext context)
        {
            this.context = context; 
        }

        public async Task<bool> LogoutAsync(int id)
        {
            var token = await context.Refreshtokens.FirstOrDefaultAsync(x=>x.UserId==id);
            
            if (token != null)
            {
                context.Refreshtokens.Remove(token);
                await context.SaveChangesAsync();
                //Dispose();
                return true;
            }
            return false;
        }
        //public void Dispose() 
        //{
        //    context.Dispose();
        //}

    }
}
