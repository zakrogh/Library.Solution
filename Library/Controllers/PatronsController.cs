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
  public class PatronsController : Controller
  {
    private readonly LibraryContext _db;

    public PatronsController(LibraryContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Patron> model = _db.Patrons.ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Patron patron)
    {
      _db.Patrons.Add(patron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
        var thisPatron = _db.Patrons
            .Include(patron => patron.Copies)
            .ThenInclude(join => join.Copy)
            .FirstOrDefault(patron => patron.PatronId == id);
        return View(thisPatron);
    }
    public ActionResult AddCopy(int id)
    {
        var thisPatron = _db.Patrons.FirstOrDefault(patrons => patrons.PatronId == id);
        ViewBag.CopyId = new SelectList(_db.Copies, "CopyId", "CopyId");
        return View(thisPatron);
    }
    [HttpPost]
    public ActionResult AddCopy(Patron patron, int CopyId)
    {
        if (CopyId != 0)
        {
        _db.PatronCopy.Add(new PatronCopy() { CopyId = CopyId, PatronId = patron.PatronId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
    public ActionResult Edit(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      return View(thisPatron);
    }

    [HttpPost]
    public ActionResult Edit(Patron patron)
    {
      _db.Entry(patron).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      return View(thisPatron);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisPatron = _db.Patrons.FirstOrDefault(patron => patron.PatronId == id);
      _db.Patrons.Remove(thisPatron);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
