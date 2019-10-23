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
  public class AuthorsController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorsController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        var userAuthors = _db.Authors.Where(entry => entry.User.Id == currentUser.Id);
        return View(userAuthors);
    }

    public ActionResult AddAuthor(int id)
    {
        var thisAuthor = _db.Authors.FirstOrDefault(books => books.AuthorId == id);
        ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
        return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult AddAuthor(Author book, int AuthorId)
    {
        if (AuthorId != 0)
        {
        _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, AuthorId = book.AuthorId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
        ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Author book, int AuthorId)
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        book.User = currentUser;
        _db.Authors.Add(book);
        if (AuthorId != 0)
        {
            _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, AuthorId = book.AuthorId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
        var thisAuthor = _db.Authors
            .Include(book => book.Authors)
            .ThenInclude(join => join.Author)
            .FirstOrDefault(book => book.AuthorId == id);
        return View(thisAuthor);
    }

    public ActionResult Edit(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(books => books.AuthorId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisAuthor);
    }

    [HttpPost]
    public ActionResult Edit(Author book, int AuthorId)
    {
      if (AuthorId != 0)
      {
        _db.AuthorBook.Add(new AuthorBook() { AuthorId = AuthorId, AuthorId = book.AuthorId });
      }
      _db.Entry(book).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(books => books.AuthorId == id);
      return View(thisAuthor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisAuthor = _db.Authors.FirstOrDefault(books => books.AuthorId == id);
      _db.Authors.Remove(thisAuthor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteBook(int joinId)
    {
        var joinEntry = _db.AuthorBook.FirstOrDefault(entry => entry.AuthorBookId == joinId);
        _db.AuthorBook.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
  }
}
