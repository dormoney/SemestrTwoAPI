using Microsoft.AspNetCore.Mvc;
using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;
using System.Security.Claims;

namespace SemestrTwoAPI.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetAllUsers(int onPage, int page);
        Task<IActionResult> GetUserByEmail(string email);
        Task<IActionResult> Register(RegisterRequest request);
        Task<IActionResult> Login(LoginRequest request);
        Task<IActionResult> UpdateUser(int id, UpdateRequest user);
        Task<IActionResult> UpdateAccount(string id, UpdateRequest user);
        Task<IActionResult> DeleteUser(int id);
        Task<IActionResult> DeleteAccount(string id);
    }
}
