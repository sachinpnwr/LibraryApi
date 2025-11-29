namespace Library.Api.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
    }

    public class LoanSummaryDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string? BookName { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }

        public DateTime IssuedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public bool IsReturned { get; set; }
    }


    public class BookWithLoansDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public List<LoanSummaryDto> Loans { get; set; } = new List<LoanSummaryDto>();
    }
}
