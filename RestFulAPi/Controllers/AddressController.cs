using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/Address")]  
    [ApiController]
    [Authorize(Policy = "AdminOrCustomer")]
    public class AddressController : ControllerBase
    {
        private readonly IAddress addressService;
        public AddressController(IAddress _addressService)
        {
            addressService = _addressService;
        }
       
        [HttpGet]
        public async Task<IActionResult> Addresses(int Id)
        {
            try
            {
                var data = await addressService.GetAddress(Id);
                return Ok(data);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Address")]
        public async Task<IActionResult> GetAddress(int Id)
        {
            try
            {
                var data = await addressService.GetAddressId(Id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(CommonAddressModel addressModel)
        {
            try
            {
                var data = await addressService.AddAddress(addressModel);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> EditAddress([FromBody] CommonAddressModel addressModel)
        {
            try
            {
                
                var result = await addressService.EditAddress(addressModel);
                if (result)
                {
                    return Ok("Address updated successfully");
                }
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Set-Default")]
        public async Task<IActionResult> SetAsDefault(int Id )
        {
            try
            {

                var result = await addressService.SetDefaultAsync(Id);
                if (result)
                {
                    return Ok("Updated Default Address");
                }
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var result = await addressService.DeleteAddress(id);
                if (result)
                {
                    return Ok("Address deleted successfully");
                }
                return NotFound("Address not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
