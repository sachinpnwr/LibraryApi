using Library.Api.DTOs.Auth;
using Library.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req) {
            try {
                await _auth.RegisterAsync(req);
                return Ok(new { message = "Registered" });
            } catch (Exception ex) {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req) {
            try {
                var resp = await _auth.LoginAsync(req.Email, req.Password);
                return Ok(resp);
            } catch (Exception ex) {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
