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
            try
            {
                var users = await _context.Users.ToListAsync();

                if (onPage == 0 || onPage >= users.Count()) return new OkObjectResult(new
                {
                    data = new { users = users },
                    response = "Success",
                    status = true
                });

                if (onPage < 0 || page <= 0) return new BadRequestObjectResult(new
                {
                    data = new { users = new List<User> () },
                    response = "Разделение страниц по отрицательным данным невозможно или страница не может быть нулевой!",
                    status = false
                });

                double pageCount = Math.Round((double)users.Count() / onPage);
                if (page > pageCount) return new BadRequestObjectResult(new
                {
                    data = new { users = new List<User> () },
                    response = $"Вы выбрали несуществующую страницу! При делении страниц по {onPage} объекта всего получилось {pageCount} страниц!",
                    status = false
                }); ;

                var paginatedUsers = new List<User> ();
                for (int i = 0; i < onPage;)
                {
                    if (page == 1) paginatedUsers.Add(users[i]);
                     else
                    {
                        if ((onPage * (page - 1) + i) < users.Count())
                        {
                            paginatedUsers.Add(users[(onPage * (page - 1) + i)]);
                        }
                        else
                        {
                            return new OkObjectResult(new
                            {
                                data = new { users = paginatedUsers },
                                response = "Success",
                                status = true
                            });
                        }
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
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    data = new { users = new List<User> () },
                    response = $"Внутренняя ошибка сервера: {ex}",

                }) { StatusCode = 500 };
            }
            
        }

        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult(new
                {
                    data = new { users = new List<User> () },
                    response = "Email string is empty",
                    status = false
                });
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(_email => _email.Email == email);

                if (user == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        data = new { users = new List<User> () },
                        response = "User with this email does not exist!",
                        status = false
                    });
                }

                return new OkObjectResult(new
                {
                    data = new { users = user },
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult( new
                {
                    data = new { users = new List<User> () },
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(o => o.Email == request.Email))
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Email already registered!",
                        status = false
                    });
                }

                _context.Users.Add(new User
                {
                    Email = request.Email,
                    Name = request.Name,
                    Description = request.Description,
                    Password = request.Password,
                    Role = request.Role
                });

                await _context.SaveChangesAsync();

                return new OkObjectResult( new
                {
                    response = "Successfully registered!",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == request.Email && user.Password == request.Password);

                if (user == null)
                {
                    return new NotFoundObjectResult( new
                    {
                        response = "Failed to login! Check your email and password",
                        status = false
                    });
                }

                var token = _jwtService.GenerateToken(user);

                return new OkObjectResult(new
                {
                    data = new { token = token },
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> UpdateUser(int id, UpdateRequest request)
        {
            try
            {
                if (request == null || id == 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Пользователь не может быть найден по вводным данным!",
                        status = false
                    });
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Пользователь с таким ID не был найден!",
                        status = false
                    });
                }

                user.Email = request.Email;
                user.Name = request.Name;
                user.Description = request.Description;
                user.Password = request.Password;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> UpdateAccount(string id, UpdateRequest request)
        {
            try
            {
                if (request == null || id == "")
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Пользователь не может быть найден по вводным данным!",
                        status = false
                    });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
                if (user == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Пользователь с таким ID не был найден!",
                        status = false
                    });
                }

                user.Email = request.Email;
                user.Name = request.Name;
                user.Description = request.Description;
                user.Password = request.Password;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) 
                {
                    return new BadRequestObjectResult(new
                    {
                        response = "Пользователь с таким ID не был найден!",
                        status = false
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new
                {
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> DeleteAccount(string id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new
                {
                    response = "Success",
                    status = true
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    response = $"Внутренняя ошибка сервера: {ex}",
                    status = false
                })
                { StatusCode = 500 };
            }
        }
    }
}
