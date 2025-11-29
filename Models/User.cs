using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public int? RoleId { get; set; } // FK
        public Role? Role { get; set; } // Navigation property

        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
