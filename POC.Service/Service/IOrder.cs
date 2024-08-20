﻿using Poc.CommonModel.Models;
using POC.CommonModel.Models;

namespace POC.ServiceLayer.Service
{
    public interface IOrder
    {
        Task<UserValidationResult> CreateOrderAsync(CommonOrderModel order, int orderedProduct);
        Task<UserValidationResult> CreateOrderAsync(CommonTemporderTable order, int orderedProduct, string SaveAsaDraft);
    }
}
