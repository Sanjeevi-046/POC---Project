using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace POC.MVC.Pages
{
    public class TestModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet(string message, string userName)
        {
            if (message != null && userName != null)
            {
                Message = $"Hi {userName}, {message}";
            }
            else
            {
                Message = "Occured Error Try Again";
            }
        }
    }
}
