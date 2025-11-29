using Library.Api.Data;
using Library.Api.DTOs;
using Library.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LoansController(AppDbContext db) => _db = db;

        // -----------------------------
        // GET ALL LOANS (ADMIN ONLY)
        // -----------------------------
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .ToListAsync();

            var loanDtos = loans.Select(l => new LoanSummaryDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookName = l.Book?.Title,
                MemberId = l.MemberId,
                MemberName = l.Member?.FullName,
                MemberEmail = l.Member?.Email,
                IssuedAt = l.IssuedAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                IsReturned = l.IsReturned
            }).ToList();

            return Ok(loanDtos);
        }

        // -----------------------------
        // GET MY LOANS (LOGGED-IN USER)
        // -----------------------------
        [Authorize]
        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!int.TryParse(sub, out var userId))
                return Unauthorized();

            var loans = await _db.Loans
                .Where(l => l.MemberId == userId)
                .Include(l => l.Book)
                .Include(l => l.Member)
                .ToListAsync();

            var loanDtos = loans.Select(l => new LoanSummaryDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookName = l.Book?.Title,
                MemberId = l.MemberId,
                MemberName = l.Member?.FullName,
                IssuedAt = l.IssuedAt,
                DueAt = l.DueAt,
                ReturnedAt = l.ReturnedAt,
                IsReturned = l.IsReturned
            }).ToList();

            return Ok(loanDtos);
        }

        // -----------------------------
        // ISSUE BOOK (ADMIN ONLY)
        // -----------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost("issue")]
        public async Task<IActionResult> Issue([FromBody] Loan loanRequest)
        {
            var book = await _db.Books.FindAsync(loanRequest.BookId);
            if (book == null) return BadRequest("Invalid Book ID");

            var member = await _db.Users.FindAsync(loanRequest.MemberId);
            if (member == null) return BadRequest("Invalid Member/User ID");

            if (book.AvailableCopies <= 0)
                return BadRequest("No available copies");

            var alreadyIssued = await _db.Loans
                .Where(l => l.MemberId == member.Id && l.BookId == book.Id && l.ReturnedAt == null)
                .FirstOrDefaultAsync();

            if (alreadyIssued != null)
                return BadRequest("Book already issued to this user");

            var loanDurationDays = 14;

            var dueDate = loanRequest.DueAt ?? DateTime.UtcNow.AddDays(loanDurationDays);

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                IssuedAt = DateTime.UtcNow,
                DueAt = dueDate
            };

            book.AvailableCopies -= 1;

            _db.Loans.Add(loan);
            await _db.SaveChangesAsync();

            // Return issued loan info via DTO
            var loanDto = new LoanSummaryDto
            {
                Id = loan.Id,
                MemberId = loan.MemberId,
                MemberName = member.FullName,
                IssuedAt = loan.IssuedAt,
                DueAt = loan.DueAt,
                ReturnedAt = loan.ReturnedAt,
                IsReturned = loan.IsReturned
            };

            return Ok(new { message = "Book issued successfully", loan = loanDto });
        }

        // -----------------------------
        // RETURN BOOK (ADMIN ONLY)
        // -----------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost("return/{id}")]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound(new { message = "Loan not found" });

            if (loan.IsReturned)
                return BadRequest(new { message = "This book is already returned" });

            loan.ReturnedAt = DateTime.UtcNow;

            if (loan.Book != null)
                loan.Book.AvailableCopies += 1;

            await _db.SaveChangesAsync();

            var loanDto = new LoanSummaryDto
            {
                Id = loan.Id,
                MemberId = loan.MemberId,
                MemberName = loan.Member?.FullName,
                IssuedAt = loan.IssuedAt,
                DueAt = loan.DueAt,
                ReturnedAt = loan.ReturnedAt,
                IsReturned = loan.IsReturned
            };

            return Ok(new { message = "Book returned successfully", loan = loanDto });
        }
    }
}
