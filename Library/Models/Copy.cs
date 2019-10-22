namespace Library.Models
{
  public class Copy
  {
    public Copy()
    {
      this.Patrons = new HashSet<PatronCopy>();
    }
    public int CopyId { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public Patron Patron { get; set; }

    public ICollection<PatronCopy> Patrons { get;}
  }
}
