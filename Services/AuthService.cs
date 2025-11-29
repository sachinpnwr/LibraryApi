using Library.Api.Data;
using Library.Api.DTOs.Auth;
using Library.Api.Helpers;
using Library.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtSettings _jwt;
        private readonly PasswordHasher<User> _hasher;

        public AuthService(AppDbContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
            _hasher = new PasswordHasher<User>();
        }

        public async Task RegisterAsync(RegisterRequest req)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                throw new Exception("Email already exists");

            var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
            if (userRole == null)
            {
                throw new Exception("User role not found in database");
            }
            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                CreatedAt = DateTime.UtcNow,
                RoleId = userRole.Id
            };

            user.PasswordHash = _hasher.HashPassword(user, req.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new Exception("Invalid credentials");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash ?? "", password);
            if (result == PasswordVerificationResult.Failed) throw new Exception("Invalid credentials");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName ?? "")
            };
            if (user.Role != null) claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationMinutes),
                signingCredentials: creds
            );

            return new Library.Api.DTOs.Auth.LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = user.Role?.Name ?? "User",
                UserId = user.Id
            };
        }
    }
}
