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
            var users = await _userService.GetAllUsers(onPage, page);
            if (onPage == 0 || onPage >= users.Count()) return Ok(users);

            if (onPage < 0 || page <= 0) return BadRequest("Разделение страниц по отрицательным данным невозможно или страница не может быть нулевой!");

            double pageCount = Math.Round((double)users.Count() / onPage);
            if (page > pageCount) return BadRequest($"Вы выбрали несуществующую страницу! При делении страниц по {onPage} объекта всего получилось {pageCount} страниц!");

            var paginatedUsers = new List<User>();
            for (int i = 0; i < onPage;)
            {
                if (page == 1) paginatedUsers.Add(users[i]);
                else
                {
                    if ((onPage * (page - 1) + i) < users.Count())
                    {
                        paginatedUsers.Add(users[(onPage * (page - 1) + i)]);
                    }
                    else break;
                }
                i++;
            }

            return Ok(paginatedUsers);
        }

        [HttpDelete("AdminDeleteById/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);

            if (!result) return BadRequest("Пользователь с таким ID не был найден либо не существует!");
            return Ok("User successfully deleted!");
        }

        [HttpPut("AdminUpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromQuery] UpdateRequest user)
        {
            var result = await _userService.UpdateUser(id, user);

            if (!result) return BadRequest("Пользователь не был найден либо отсутвуют новые данные");

            return Ok("User successfully updated!");
        }

    }
}
