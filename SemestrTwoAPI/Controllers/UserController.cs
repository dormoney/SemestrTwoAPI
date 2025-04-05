using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Interfaces;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;

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
            var result = await _userService.Register(request);
            if (!result) return BadRequest("Email already registered!");
            return Ok("Successfully registered!");
        }

        [Authorize]
        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null) return BadRequest("User with this email does not exist!");

            return Ok(user);
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequest request)
        {
            var token = await _userService.Login(request);
            if (token == "false") return BadRequest("Failed to login! Check your email and password");

            HttpContext.Response.Cookies.Append("jwt", token);
            return Ok("Successfully logged in!");
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");

            return Ok("Successfully logged out!");
        }
    }
}
