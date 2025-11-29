using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Library.Api.Models {
    public class Role {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public ICollection<User>? Users { get; set; }
    }
}
