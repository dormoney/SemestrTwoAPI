
using System.ComponentModel.DataAnnotations;

namespace SemestrTwoAPI.Requests
{
    public class RegisterRequest
    {
        [Required][EmailAddress] public string Email { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required] public string Password { get; set; }
        public string _isAdmin { get; set; }
    }
}
