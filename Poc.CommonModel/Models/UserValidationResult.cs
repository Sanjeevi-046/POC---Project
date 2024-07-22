﻿namespace Poc.CommonModel.Models
{
    public class UserValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string? Mail { get; set; }
        public string? Role { get; set; }
        public int? userId { get; set; }
    }
}
