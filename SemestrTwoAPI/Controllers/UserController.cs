using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Interfaces;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;
using System.Security.Claims;

namespace SemestrTwoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromQuery] RegisterRequest request)
        {
            return await _userService.Register(request);
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequest request)
        {
            return await _userService.Login(request);

        }

        [Authorize]
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromQuery] UpdateRequest request)
        {
            var userId = User.FindFirst("Id")?.Value;
            return await _userService.UpdateAccount(userId, request);
        }

        [Authorize]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst("Id")?.Value;
            return await _userService.DeleteAccount(userId);
        }
    }
}
