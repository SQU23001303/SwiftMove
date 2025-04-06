using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swift_Move.Data;
using Swift_Move.Models;

namespace Swift_Move.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var services = _context.Services
                .Include(s => s.ServiceStaff)
                .ThenInclude(ss => ss.Staff)
                .ToList();

            return View(services);
        }

        public IActionResult Assign(int id)
        {
            var service = _context.Services
                .Include(s => s.ServiceStaff)
                .ThenInclude(ss => ss.Staff)
                .FirstOrDefault(s => s.Id == id);

            ViewBag.StaffList = _context.Staff.ToList();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(ServiceModel model, List<int> SelectedStaffIds)
        {
            var service = _context.Services
                .Include(s => s.ServiceStaff)
                .FirstOrDefault(s => s.Id == model.Id);

            if (service == null)
                return NotFound();

            // Update quote price
            service.QuotePrice = model.QuotePrice;

            // Remove existing staff
            var existingAssignments = _context.ServiceStaff
                .Where(ss => ss.ServiceModelId == service.Id);
            _context.ServiceStaff.RemoveRange(existingAssignments);

            // Add selected staff (up to 3)
            var selected = SelectedStaffIds.Take(3).ToList();
            foreach (var staffId in selected)
            {
                _context.ServiceStaff.Add(new ServiceStaff
                {
                    ServiceModelId = service.Id,
                    StaffId = staffId
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
