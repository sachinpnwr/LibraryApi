using Library.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Api.Data
{
    public static class SeedData
    {
        public static void Seed(AppDbContext db, IServiceProvider services)
        {

            // ---------- ROLES ----------
            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Student" }
                );
                db.SaveChanges();
            }

            // ---------- ADMIN USER ----------
            if (!db.Users.Any(u => u.Email == "admin@library.local"))
            {
                var hasher = new PasswordHasher<User>();
                var adminRole = db.Roles.First(r => r.Name == "Admin");

                var admin = new User
                {
                    FullName = "Admin",
                    Email = "admin@library.local",
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow
                };

                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

                db.Users.Add(admin);
                db.SaveChanges();
            }

            // ---------- 10 STUDENTS ----------
            var studentRoleId = db.Roles.First(r => r.Name == "Student").Id;

            if (!db.Users.Any(u => u.RoleId == studentRoleId))
            {

                var studentRole = db.Roles.First(r => r.Name == "Student");
                var hasher = new PasswordHasher<User>();

                var mockNames = new List<string>
                {
                    "John Carter",
                    "Emily Thompson",
                    "Michael Anderson",
                    "Sophia Wilson",
                    "Daniel Smith",
                    "Olivia Brown",
                    "Lucas Martin",
                    "Emma Johnson",
                    "James Miller",
                    "Ava Davis"
                };

                int index = 1;
                foreach (var name in mockNames)
                {
                    var student = new User
                    {
                        FullName = name,
                        Email = $"student{index}@library.local",
                        RoleId = studentRole.Id,
                        CreatedAt = DateTime.UtcNow
                    };

                    student.PasswordHash = hasher.HashPassword(student, "Student@123");

                    db.Users.Add(student);
                    index++;
                }

                db.SaveChanges();
            }

            // ---------- 50 BOOKS ----------
            if (!db.Books.Any())
            {

                var books = new List<Book>
                {
                    new Book { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884" },
                    new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "9780201616224" },
                    new Book { Title = "Clean Architecture", Author = "Robert C. Martin", Isbn = "9780134494166" },
                    new Book { Title = "Design Patterns", Author = "GoF", Isbn = "9780201633610" },
                    new Book { Title = "Refactoring", Author = "Martin Fowler", Isbn = "9780201485677" },
                    new Book { Title = "Introduction to Algorithms", Author = "CLRS", Isbn = "9780262033848" },
                    new Book { Title = "JavaScript: The Good Parts", Author = "Douglas Crockford", Isbn = "9780596517748" },
                    new Book { Title = "You Don't Know JS", Author = "Kyle Simpson", Isbn = "9781491904244" },
                    new Book { Title = "Eloquent JavaScript", Author = "Marijn Haverbeke", Isbn = "9781593279509" },
                    new Book { Title = "Head First Java", Author = "Kathy Sierra", Isbn = "9780596009205" },

                    new Book { Title = "Head First Design Patterns", Author = "Eric Freeman", Isbn = "9781492078005" },
                    new Book { Title = "C# in Depth", Author = "Jon Skeet", Isbn = "9781617294532" },
                    new Book { Title = "Pro ASP.NET Core", Author = "Adam Freeman", Isbn = "9781484254394" },
                    new Book { Title = "Learning Python", Author = "Mark Lutz", Isbn = "9781449355739" },
                    new Book { Title = "Python Crash Course", Author = "Eric Matthes", Isbn = "9781593276034" },

                    new Book { Title = "Fluent Python", Author = "Luciano Ramalho", Isbn = "9781492056355" },
                    new Book { Title = "Effective Java", Author = "Joshua Bloch", Isbn = "9780134685991" },
                    new Book { Title = "Operating System Concepts", Author = "Silberschatz", Isbn = "9781119456339" },

                    new Book { Title = "Computer Networks", Author = "Andrew S. Tanenbaum", Isbn = "9780132126953" },
                    new Book { Title = "The Mythical Man-Month", Author = "Frederick Brooks", Isbn = "9780201835953" },

                    new Book { Title = "The Phoenix Project", Author = "Gene Kim", Isbn = "9781942788294" },
                    new Book { Title = "The DevOps Handbook", Author = "Gene Kim", Isbn = "9781942788003" },
                    new Book { Title = "Soft Skills", Author = "John Sonmez", Isbn = "9781617292392" },

                    new Book { Title = "Peopleware", Author = "Tom DeMarco", Isbn = "9780932633439" },
                    new Book { Title = "Working Effectively with Legacy Code", Author = "Michael Feathers", Isbn = "9780131177055" },

                    new Book { Title = "Cracking the Coding Interview", Author = "Gayle Laakmann", Isbn = "9780984782857" },
                    new Book { Title = "System Design Interview", Author = "Alex Xu", Isbn = "9781736049119" },
                    new Book { Title = "Grokking Algorithms", Author = "Aditya Bhargava", Isbn = "9781617292231" },

                    new Book { Title = "Deep Work", Author = "Cal Newport", Isbn = "9781455586691" },
                    new Book { Title = "Atomic Habits", Author = "James Clear", Isbn = "9780735211292" },

                    // 20 more filler books (auto-generate)
                };

                // Auto-generate 20 filler books
                for (int i = 1; i <= 20; i++)
                {
                    books.Add(new Book
                    {
                        Title = $"Sample Book {i}",
                        Author = $"Author {i}",
                        Isbn = $"97800000000{i}",
                    });
                }

                // Set default copies
                foreach (var b in books)
                {
                    b.TotalCopies = 5;
                    b.AvailableCopies = 5;
                }

                db.Books.AddRange(books);
                db.SaveChanges();
            }
        }
    }
}
