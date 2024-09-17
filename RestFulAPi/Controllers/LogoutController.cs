using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/Logout")]
    [ApiController]
    
    public class LogoutController : ControllerBase
    {
        private readonly ILogout logoutService;
        public LogoutController(ILogout logoutService)
        {
            this.logoutService=logoutService;
        }
        [HttpGet]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> Logout(int Id)
        {
            var Logout = await logoutService.Logout(Id);
            if (Logout)
            {
                return Ok();
            }
            return BadRequest("Error Occured During LogOut");
        }

    }
}
