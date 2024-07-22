using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.Repository
{
    public interface ICartRepo
    {
        Task<List<CommonCartModel>> GetCartAsync(int id);
    }
}
