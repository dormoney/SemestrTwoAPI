﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemestrTwoAPI.DataBaseContext;
using SemestrTwoAPI.Interfaces;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;

namespace SemestrTwoAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ContextDB _context;
        private readonly IJwtService _jwtService;

        public UserService(ContextDB context, IJwtService jwtService) {
            _context = context; 
            _jwtService = jwtService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return null;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(_email => _email.Email == email);
            return user;
        }

        public async Task<bool> Register(User user)
        {
            if (await _context.Users.AnyAsync(o => o.Email == user.Email)) return false;
            _context.Users.Add(user);
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

        public async Task<bool> UpdateUser(User user)
        {
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            return true;
        }
    }
}
