using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize(Policy = "AdminOrCustomer")]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly ILogger<UserController> logger;
        public UserController(IUser userService, ILogger<UserController> logger)
        {
            _userService = userService;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(int Id)
        {
            try
            {
                var data = await _userService.GetUser(Id);
                if (data == null) { 
                    return NotFound("Not Found");
                }

                return Ok(data);
            }
            catch (Exception ex) {
                logger.LogError(ex.ToString());
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]CommonUserModel commonUserModel )
        {
            try
            {
                var data = await _userService.UpdateUser(commonUserModel);
                if (data.IsValid)
                {
                    return Ok(data);
                }
                return BadRequest(data.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
