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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ServiceModel service)
    {
        if (ModelState.IsValid)
        {
            try
            {
                service.CollectionDate = service.CollectionDate.ToUniversalTime();
                service.DeliveryDate = service.DeliveryDate.ToUniversalTime();

                _context.Service.Add(service);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the service. Please try again.");
            }
        }

        return View(service);
    }


}