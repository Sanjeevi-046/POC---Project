using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceLayer.Service;
using System.IO;

namespace POC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    //[Authorize(Policy = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _adminService;
        public AdminController(IAdmin adminService) 
        {
            _adminService = adminService;   
        }
       
        [HttpGet]
        public async Task<IActionResult> OrderDetail(string sortColumnName = "Order ID",string sortOrder = "asc", string Status ="" , string SearchName ="", DateTime? fromDate = null, DateTime? toDate = null, int Page = 1)
        {
            var data = await _adminService.GetOrderAsync(sortColumnName, sortOrder, Status,SearchName,fromDate,toDate,Page);
            return Ok( data );
        }
        [HttpGet("download-invoice")]
        public async Task<IActionResult> DownloadInvoice(int Id)
        {
            var pdfStream = await _adminService.GetInvoicePdfAsync(Id);

            if (pdfStream == null)
            {
                return NotFound();
            }

            var fileName = $"Invoice_{Id}.pdf";
            var contentType = "application/pdf";

            return File(pdfStream, contentType, fileName);
        }
        [HttpGet("download-Excel")]
        public async Task<IActionResult> DownloadExcel()
        {
            var Stream = await _adminService.DownloadExcel();

            if (Stream != null)
            {
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
         
                return File(Stream, contentType, "Invoice.xlsx");
            }
            return NotFound();
        }

    }
}
