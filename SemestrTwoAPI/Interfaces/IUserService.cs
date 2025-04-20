using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;
using System.Security.Claims;

namespace SemestrTwoAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByEmail(string email);
        Task<bool> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        Task<bool> UpdateUser(int id, UpdateRequest user);
        Task<bool> UpdateAccount(string id, UpdateRequest user);
        Task<bool> DeleteUser(int id);
        Task<bool> DeleteAccount(string id);
    }
}
