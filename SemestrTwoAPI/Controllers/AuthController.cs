using SemestrTwoAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Services;

namespace SemestrTwoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Здесь должна быть проверка логина и пароля в базе данных.
            // Для примера допустим, что пользователь "admin" существует.
            if (request.Email == "admin" && request.Password == "admin123")
            {
                // Генерируем токен (роль "Admin" для примера)
                var token = _jwtService.GenerateToken(request.Email, "Admin");
                return Ok(new { Token = token });
            }

            return Unauthorized("Неверный логин или пароль");
        }
    }
}
