using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.TempModel
{
    public class UserValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string ?Mail { get; set; }
        public string? Role { get; set; }
        public int? userId { get; set; }
    }
}
