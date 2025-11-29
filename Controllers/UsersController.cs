using System.Security.Claims;
using Library.Api.Data;
using Library.Api.DTOs;
using Library.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) { _db = db; }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Include(u => u.Role).ToListAsync();

            var dtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role?.Name
            }).ToList();

            return Ok(dtos);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            var loggedInUserId = int.Parse(User.FindFirst("sub")!.Value);
            var loggedInRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // ❌ If not admin AND updating someone else → not allowed
            if (!string.Equals(loggedInRole, "Admin", StringComparison.OrdinalIgnoreCase)
      && loggedInUserId != id)
            {
                return Unauthorized("You are not allowed to update other users.");
            }

            // ❌ Admin updating another Admin → not allowed
            // if (loggedInRole == "Admin" && user.Role?.Name == "Admin" && loggedInUserId != id)
            //     return Unauthorized("Admin cannot update another Admin.");

            // 🔄 Update Full Name
            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            // 🔄 Update Email (must be unique)
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
                if (emailExists)
                    return BadRequest("Email is already used by another user.");

                user.Email = dto.Email;
            }

            // 🔄 Update Role (admin only)
            if (!string.IsNullOrEmpty(dto.Role))
            {
                if (loggedInRole != "Admin")
                    return Unauthorized("Only Admin can update roles.");

                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role);
                if (role == null)
                    return BadRequest("Role not found");

                user.RoleId = role.Id;
            }

            // 🔄 Update Password
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, dto.Password);
            }

            await _db.SaveChangesAsync();

            var updated = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

            var result = new UserDto
            {
                Id = updated.Id,
                FullName = updated.FullName,
                Email = updated.Email,
                Role = updated.Role?.Name
            };

            return Ok(result);
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!int.TryParse(sub, out var userId)) return Unauthorized();
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();
            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                Role = user.Role?.Name
            });
        }
    }
}
