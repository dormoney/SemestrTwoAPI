using SemestrTwoAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace SemestrTwoAPI.Controllers
{
    [Authorize]
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
            return null;
        }
    }
}
