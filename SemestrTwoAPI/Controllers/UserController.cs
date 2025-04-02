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
        private HttpContext _context;

        public UserController(IUserService userService, HttpContext context)
        {
            _userService = userService;
            _context = context;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(User user)
        {
            var result = await _userService.Register(user);
            if (!result) return BadRequest("Email already registered!");
            return Ok();
        }

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

            _context.Response.Cookies.Append("jwt", token);
            return Ok();
        }
    }
}
