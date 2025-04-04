using SemestrTwoAPI.Model;
using SemestrTwoAPI.Requests;

namespace SemestrTwoAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByEmail(string email);
        Task<bool> Register(RegisterRequest re);
        Task<string> Login(LoginRequest request);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
