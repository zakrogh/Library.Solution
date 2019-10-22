using System.Collections.Generic;

namespace Library.Models
{
    public class Book
    {
        public Book()
        {
            this.Patrons = new HashSet<PatronBook>();
        }

        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ICollection<PatronBook> Patrons { get;}
    }
}
