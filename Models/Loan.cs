using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library.Api.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [Required]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public User? Member { get; set; }  // renamed from User -> Member

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public bool IsReturned => ReturnedAt != null;
    }
}
