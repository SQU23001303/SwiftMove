using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swift_Move.Data;
using Swift_Move.Models;

public class ServiceController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult PublicList()
    {
        var services = _context.ServiceList.ToList();
        return View(services);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var services = _context.ServiceList.ToList();
        return View(services);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(ServiceList service)
    {
        if (ModelState.IsValid)
        {
            _context.ServiceList.Add(service);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(service);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var service = _context.ServiceList.Find(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(ServiceList service)
    {
        if (ModelState.IsValid)
        {
            _context.ServiceList.Update(service);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(service);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var service = _context.ServiceList.Find(id);
        return service == null ? NotFound() : View(service);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var service = _context.ServiceList.Find(id);
        if (service != null)
        {
            _context.ServiceList.Remove(service);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
