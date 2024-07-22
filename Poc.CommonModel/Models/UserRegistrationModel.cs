using POC.CommonModel.Models;

namespace Poc.CommonModel.Models
{
    public class UserRegistrationModel
    {
        public string rePassword { get; set; }
        public CommonLoginModel Login { get; set; }
    }
}
