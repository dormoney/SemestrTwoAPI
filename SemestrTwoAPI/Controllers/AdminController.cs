using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Interfaces;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;

namespace SemestrTwoAPI.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            return await _userService.GetUserByEmail(email); 
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int onPage, int page)
        {
            return await _userService.GetAllUsers(onPage, page);
        }

        [HttpDelete("AdminDeleteById/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return await _userService.DeleteUser(id);
        }

        [HttpPut("AdminUpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromQuery] UpdateRequest user)
        {
            return await _userService.UpdateUser(id, user);
        }

    }
}
