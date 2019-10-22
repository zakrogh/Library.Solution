using System.Collections.Generic;

namespace Library.Models
{
  public class Patron
    {
        public Patron()
        {
            this.Books = new HashSet<PatronBook>();
        }

        public int PatronId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PatronBook> Books { get; set; }
    }
}
