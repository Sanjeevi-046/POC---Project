﻿using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;

namespace POC.ServiceLayer.Service
{
    public class ProductService : IProduct
    {
        private readonly IProductRepo _productRepoService;

        public ProductService(IProductRepo productRepo)
        {
            _productRepoService = productRepo;
        }

        public async Task<CommonPaginationModel> GetAllProductsAsync(int page, string searchName)
        {
            var ProductData = await _productRepoService.GetAllProducts(page, searchName);
            return ProductData;
        }
        public async Task<CommonProductModel> GetProductByIdAsync(int id)
        {
            var ProductData = await _productRepoService.GetProductById(id);
            return ProductData;
        }
        public async Task<UserValidationResult> AddProductAsync(CommonProductModel product)
        {
            var ProductData = await _productRepoService.AddProduct(product);
            return ProductData;

        }
        public async Task<MemoryStream> DownloadExcel()
        {
            var Files = await _productRepoService.DownloadExcelFile();
            return Files;
        }
        public async Task<MemoryStream> DownloadHtml()
        {
            var Files = await _productRepoService.DownloadHtmlReport();
            return Files;
        }
    }
}
