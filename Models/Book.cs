using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Library.Api.Models {
    public class Book {
        [Key]
        public int Id { get; set; }
        [Required] public string Title { get; set; } = "";
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public int TotalCopies { get; set; } = 1;
        public int AvailableCopies { get; set; } = 1;
        [JsonIgnore]
        public ICollection<Loan> Loans { get; set; }
    }
}
