using Microsoft.EntityFrameworkCore;
using POC.CommonModel.Models;

namespace POC.DataLayer.Context
{

    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminOrderDetails> AdminOrderDetails { get; set; }
       
    }
}
