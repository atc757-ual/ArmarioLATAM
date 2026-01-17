using AuthService.API.Models;
using System.Threading.Tasks;

namespace AuthService.API.Services
{
    public interface IAuthService
    {
        Task<User?> Register(string email, string password, string name, string passport);
        Task<User?> LoginAndGetUser(string email, string password);
        Task<string?> Login(string email, string password);
        string GenerateJwt(User user);
    }
}
