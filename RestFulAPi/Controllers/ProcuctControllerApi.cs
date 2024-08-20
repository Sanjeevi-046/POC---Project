using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;
using System.Net.Http.Headers;

namespace POC.Api.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProcuctControllerApi : ControllerBase
    {
        private readonly IProduct _productService;
        
        public ProcuctControllerApi(IProduct productService )
        {
            _productService = productService;
        }

        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("Products")]
        public async Task<IActionResult> GetProductList(int page =1, string searchTerm="")
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(page, searchTerm);
                return Ok(products);
            }
            catch (Exception ex) 
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("Product")]
        public async Task<ActionResult<CommonProductModel>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    Exception exception = new Exception("Error");
                    CustomFileLogger.LogError("An error occurred while processing your request.", exception);
                    return StatusCode(500, "Internal server error");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(CommonProductModel product)
        {
            try
            {
                var result = await _productService.AddProductAsync(product);
                if (result.IsValid)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("ExportProductsToExcel")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            try
            {
                var stream = await _productService.DownloadExcel();
                if (stream != null)
                {
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = "Products.xlsx";
                    Response.Headers.Add("Content-Disposition", new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    }.ToString());
                    Response.Headers.Add("Content-Length", stream.Length.ToString());
                    Response.ContentType = contentType;
                    return File(stream, contentType, fileName);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }

        }
        [Authorize(Policy = "AdminOrCustomer")]
        [HttpGet("ExportProductsToHtml")]
        public async Task<IActionResult> ExportProductsToHtml()
        {
            try
            {
                var stream = await _productService.DownloadHtml();
                if (stream != null)
                {
                    var contentType = "text/html";
                    var fileName = "Products.html";
                    Response.Headers.Add("Content-Disposition", new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    }.ToString());
                    Response.Headers.Add("Content-Length", stream.Length.ToString());
                    Response.ContentType = contentType;
                    return File(stream, contentType, fileName);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
