using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
  {
    public virtual DbSet<Patron> Patrons { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<PatronBook> PatronBook { get; set; }

    public LibraryContext(DbContextOptions options) : base(options) { }
  }
}
