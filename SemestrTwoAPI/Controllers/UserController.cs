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
            var result = await _userService.Register(request);
            if (!result) return BadRequest("Email already registered!");
            return Ok("Successfully registered!");
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequest request)
        {
            var token = await _userService.Login(request);
            if (token == "false") return BadRequest("Failed to login! Check your email and password");

            HttpContext.Response.Cookies.Append("jwt", token);
            return Ok("Successfully logged in!");
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");

            return Ok("Successfully logged out!");
        }

        [Authorize]
        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount([FromQuery] UpdateRequest request)
        {
            var userId = User.FindFirst("Id")?.Value;
            if (userId == null) return BadRequest("Чтобы обновить аккаунт нужно сначала авторизоваться");

            var result = await _userService.UpdateAccount(userId, request);
            if (!result) return BadRequest("Пользователь с таким ID не был найден либо не существует!");
            return Ok("Account info successfuly updated!");
        }

        [Authorize]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst("Id")?.Value;
            if (userId == null) return BadRequest("Чтобы удалить аккаунт нужно сначала авторизоваться");

            var result = await _userService.DeleteAccount(userId);
            if (!result) return BadRequest("Пользователь с таким ID не был найден либо не существует!");
            return Ok("You deleted an account( We wish to see you later!");
        }
    }
}
