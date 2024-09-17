using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using POC.MVC.Authorization;
using POC.MVC.Models;
using POC.CommonModel.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace POC.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AuthorizedRequest _authorizedRequest;
        public AdminController(AuthorizedRequest authorizedRequest)
        {
            _authorizedRequest = authorizedRequest;
        }

        public async Task<IActionResult> Index(string sortColumnName = "Order ID", string sortOrder = "asc", string searchName = "", string Status = "", DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
        {
            ViewBag.searchName = searchName;
            ViewBag.Status = Status;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.CurrentSortOrder =sortOrder;
            ViewBag.CurrentSortColumn =sortColumnName;
            var response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Admin?sortColumnName={sortColumnName}&sortOrder={sortOrder}&Status={Status}&SearchName={searchName}&fromDate={fromDate}&toDate={toDate}&Page={page}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var JsonData = JObject.Parse(data);
                ViewBag.TotalPages = JsonData["total"].ToString();
                ViewBag.CurrentPage = JsonData["page"].ToString();
                var OrderData = JsonData["adminOrderDetails"].ToString();
                var products = JsonConvert.DeserializeObject<List<AdminOrderDetails>>(OrderData);

                return View(products);
            }
            return View();
        }

        //download-invoice?Id=
        public async Task<IActionResult> Invoice(int Id)
        {
            var response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Admin/download-invoice?Id={Id}");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.ToString();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    var filename = contentDisposition.FileName;
                    return File(stream, contentType, filename);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
            }
        }
        //download-Excel
        public async Task<IActionResult> Download()
        {
            var response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Admin/download-Excel");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.ToString();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    var filename = contentDisposition.FileName;
                    return File(stream, contentType, filename);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
            }
        }
    }
}
