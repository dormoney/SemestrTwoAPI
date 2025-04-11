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

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(_email => _email.Email == email);
            return user;
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
