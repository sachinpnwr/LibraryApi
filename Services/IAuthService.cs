using Library.Api.DTOs.Auth;

namespace Library.Api.Services {
    public interface IAuthService {
        Task<Library.Api.DTOs.Auth.LoginResponse> LoginAsync(string email, string password);
        Task RegisterAsync(RegisterRequest req);
    }
}
