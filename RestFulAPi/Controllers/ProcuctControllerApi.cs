using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using POC.DataAccess.Service;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System.Net;
using System.Net.Http.Headers;

namespace POC.Api.Controllers
{
    [Route("Product")]
    [ApiController]
    
    public class ProcuctControllerApi : ControllerBase
    {
        private readonly IProduct _productService;
        public ProcuctControllerApi(IProduct productService)
        {
            _productService = productService;

        }
        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("getProductList")]
        public async Task<IActionResult> getProductList()
        {
            var products = await _productService.GetAllProductsAsync();
            
            return Ok(products);
        }
        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("getProductById")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var result = await _productService.addProduct(product);
            if (result.IsValid)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("ExportProductsToExcel")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            var stream = await _productService.DownloadExcel();
            if (stream != null)
            {
                // Set content type and headers for the response
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Products.xlsx";

                // Set response headers
                Response.Headers.Add("Content-Disposition", new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                }.ToString());
                Response.Headers.Add("Content-Length", stream.Length.ToString());
                Response.ContentType = contentType;

                // Return the file as FileStreamResult
                return File(stream, contentType, fileName);
            }
            return NotFound();

        }



    }
}
