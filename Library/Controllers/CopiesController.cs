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
  public class CopiesController : Controller
  {
    private readonly LibraryContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CopiesController(UserManager<ApplicationUser> userManager, LibraryContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        var userCopies = _db.Copies.Where(entry => entry.User.Id == currentUser.Id);
        return View(userCopies);
    }

    public ActionResult AddPatron(int id)
    {
        var thisCopy = _db.Copies.FirstOrDefault(copys => copys.CopyId == id);
        ViewBag.PatronId = new SelectList(_db.Patrons, "PatronId", "Name");
        return View(thisCopy);
    }

    [HttpPost]
    public ActionResult AddPatron(Copy copy, int PatronId)
    {
        if (PatronId != 0)
        {
        _db.PatronCopy.Add(new PatronCopy() { PatronId = PatronId, CopyId = copy.CopyId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Create()
    {
        ViewBag.BookId = new SelectList(_db.Books, "BookId", "Title");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Copy copy, int PatronId)
    {
        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUser = await _userManager.FindByIdAsync(userId);
        copy.User = currentUser;
        _db.Copies.Add(copy);
        if (PatronId != 0)
        {
            _db.PatronCopy.Add(new PatronCopy() { PatronId = PatronId, CopyId = copy.CopyId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
        var thisCopy = _db.Copies
            .Include(copy => copy.Patrons)
            .ThenInclude(join => join.Patron)
            .FirstOrDefault(copy => copy.CopyId == id);
        return View(thisCopy);
    }

    public ActionResult Edit(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copys => copys.CopyId == id);
      ViewBag.PatronId = new SelectList(_db.Patrons, "PatronId", "Name");
      return View(thisCopy);
    }

    [HttpPost]
    public ActionResult Edit(Copy copy, int PatronId)
    {
      if (PatronId != 0)
      {
        _db.PatronCopy.Add(new PatronCopy() { PatronId = PatronId, CopyId = copy.CopyId });
      }
      _db.Entry(copy).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copys => copys.CopyId == id);
      return View(thisCopy);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisCopy = _db.Copies.FirstOrDefault(copys => copys.CopyId == id);
      _db.Copies.Remove(thisCopy);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeletePatron(int joinId)
    {
        var joinEntry = _db.PatronCopy.FirstOrDefault(entry => entry.PatronCopyId == joinId);
        _db.PatronCopy.Remove(joinEntry);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
  }
}
