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
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BooksController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Books
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _db.Books.ToListAsync();
            var dtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Isbn = b.Isbn,
                TotalCopies = b.TotalCopies,
                AvailableCopies = b.AvailableCopies
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/Books/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> Get(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Isbn = book.Isbn,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies
            };
            return Ok(dto);
        }

        [Authorize]
        [HttpGet("{id}/loans")]
        public async Task<ActionResult<BookWithLoansDto>> GetBookWithLoans(int id)
        {
            var book = await _db.Books
                .Include(b => b.Loans)
                .ThenInclude(l => l.Member) // assumes each loan has a Member object
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            var dto = new BookWithLoansDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Isbn = book.Isbn,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Loans = book.Loans.Select(l => new LoanSummaryDto
                {
                    Id = l.Id,
                    MemberId = l.MemberId,
                    MemberName = l.Member?.FullName,
                    IssuedAt = l.IssuedAt,
                    DueAt = l.DueAt,
                    ReturnedAt = l.ReturnedAt,
                    IsReturned = l.IsReturned
                }).ToList()
            };
            return Ok(dto);
        }


        // POST: api/Books
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BookDto>> Add(BookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Isbn = dto.Isbn,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.AvailableCopies
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            dto.Id = book.Id;
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        // PUT: api/Books/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookDto dto)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Isbn = dto.Isbn;
            book.TotalCopies = dto.TotalCopies;
            book.AvailableCopies = dto.AvailableCopies;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Books/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
