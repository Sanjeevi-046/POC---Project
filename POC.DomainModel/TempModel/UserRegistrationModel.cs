using POC.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.TempModel
{
    public class UserRegistrationModel
    {
        public string rePassword { get; set; }
        public Login Login { get; set; }
    }
}
