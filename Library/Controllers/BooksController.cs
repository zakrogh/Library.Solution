using Microsoft.AspNetCore.Mvc;
using Library.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Library.Controllers
{
  [Authorize]
  public class BooksController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        var userBooks = _db.Books.Where(entry => entry.User.Id == currentUser.Id);
        return View(userBooks);
    }

    public ActionResult AddPatron(int id)
    {
        var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
        ViewBag.PatronId = new SelectList(_db.Patrons, "PatronId", "Name");
        return View(thisBook);
    }

    [HttpPost]
    public ActionResult AddPatron(Book book, int PatronId)
    {
        if (PatronId != 0)
        {
        _db.PatronBook.Add(new PatronBook() { PatronId = PatronId, BookId = book.BookId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
        ViewBag.PatronId = new SelectList(_db.Patrons, "PatronId", "Name");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Book book, int PatronId)
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        book.User = currentUser;
        _db.Books.Add(book);
        if (PatronId != 0)
        {
            _db.PatronBook.Add(new PatronBook() { PatronId = PatronId, BookId = book.BookId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
        var thisBook = _db.Books
            .Include(book => book.Patrons)
            .ThenInclude(join => join.Patron)
            .FirstOrDefault(book => book.BookId == id);
        return View(thisBook);
    }

    public ActionResult Edit(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
      ViewBag.PatronId = new SelectList(_db.Patrons, "PatronId", "Name");
      return View(thisBook);
    }

    [HttpPost]
    public ActionResult Edit(Book book, int PatronId)
    {
      if (PatronId != 0)
      {
        _db.PatronBook.Add(new PatronBook() { PatronId = PatronId, BookId = book.BookId });
      }
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
      return View(thisBook);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisBook = _db.Books.FirstOrDefault(books => books.BookId == id);
      _db.Books.Remove(thisBook);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeletePatron(int joinId)
    {
        var joinEntry = _db.PatronBook.FirstOrDefault(entry => entry.PatronBookId == joinId);
        _db.PatronBook.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
  }
}
