using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemestrTwoAPI.DataBaseContext;
using SemestrTwoAPI.Interfaces;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;
using System.Security.Claims;

namespace SemestrTwoAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ContextDB _context;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ContextDB context, IJwtService jwtService, IHttpContextAccessor httpContextAccessor) {
            _context = context; 
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> GetAllUsers(int onPage, int page)
        {
            var users = await _context.Users.ToListAsync();

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

            return new OkObjectResult(new
            {
                data = new { users = users },
                response = "Success",
                status = true
            });
        }

        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult(new
                {
                    data = new { users = new User { } },
                    response = "Email string is empty",
                    status = false
                });
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(_email => _email.Email == email);

                if (user == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        data = new { users = new User { } },
                        response = "User with this email does not exist!",
                        status = false
                    });
                }

                return new OkObjectResult(new
                {
                    data = new { user = user },
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult( new
                {
                    data = new { users = new { } },
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = true
                }) { StatusCode = 500 };
            }
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(o => o.Email == request.Email)) return false;
            _context.Users.Add(new User 
            {
                Email = request.Email,
                Name = request.Name,
                Description = request.Description,
                Password = request.Password,
                Role = request.Role
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == request.Email && user.Password == request.Password);

            if (user == null) return "false";

            var token = _jwtService.GenerateToken(user);

            return token;
        }

        public async Task<bool> UpdateUser(int id, UpdateRequest request)
        {
            if (request == null || id == 0) return false;

            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Email = request.Email;
            user.Name = request.Name;
            user.Description = request.Description;
            user.Password = request.Password;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAccount(string id, UpdateRequest request)
        {
            if (request == null || id == "") return false;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user == null) return false;

            user.Email = request.Email;
            user.Name = request.Name;
            user.Description = request.Description;
            user.Password = request.Password;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccount(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
