using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swift_Move.Data;
using Swift_Move.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Swift_Move.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Services()
        {
            var dynamicServices = _context.ServiceList.ToList();
            return View(dynamicServices);
        }

        public IActionResult Bookings()
        {
            ViewBag.ServiceOptions = _context.ServiceList
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Title            
                }).ToList();
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // 2. Assign the ID to the booking
                service.UserId = userId;

                try
                {
                    service.CollectionDate = service.CollectionDate.ToUniversalTime();
                    service.DeliveryDate = service.DeliveryDate.ToUniversalTime();

                    _context.Services.Add(service);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the service. Please try again.");
                }
            }

            ViewBag.ServiceOptions = _context.ServiceList
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Title
                }).ToList();

            return View("Bookings", service);
        }


        [Authorize]
        public IActionResult Portal()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var bookings = _context.Services
                .Where(s => s.UserId == userId)
                .Include(s => s.ServiceStaff)
                .ThenInclude(ss => ss.Staff)
                .ToList();

            decimal totalSpent = bookings
                .Where(b => b.QuotePrice.HasValue)
                .Sum(b => b.QuotePrice.Value);

            string loyaltyLevel = totalSpent switch
            {
                >= 1000 => "Gold",
                >= 500 => "Silver",
                _ => "Bronze"
            };

            ViewBag.LoyaltyLevel = loyaltyLevel;
            ViewBag.TotalSpent = totalSpent;

            var bookedMonths = bookings
                .Select(b => b.CollectionDate.ToString("yyyy-MM"))
                .Distinct()
                .Count();

            bool freeMonthUnlocked = bookedMonths >= 11;

            ViewBag.FreeMonthUnlocked = freeMonthUnlocked;
            ViewBag.MonthsBooked = bookedMonths;

            return View(bookings);
        }

        [Authorize]
        public IActionResult EditBooking(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var service = _context.Services
                .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (service == null)
            {
                return NotFound();
            }

            ViewBag.ServiceOptions = _context.ServiceList
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Title
                }).ToList();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult EditBooking(ServiceModel updatedService)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var existing = _context.Services
                .Include(s => s.ServiceStaff)
                .FirstOrDefault(s => s.Id == updatedService.Id && s.UserId == userId);

            ViewBag.ServiceOptions = _context.ServiceList
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Title
                }).ToList();

            if (existing == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update editable fields
                existing.Title = updatedService.Title;
                existing.CollectionAddress = updatedService.CollectionAddress;
                existing.DeliveryAddress = updatedService.DeliveryAddress;
                existing.ServiceList = updatedService.ServiceList;
                existing.CollectionDate = updatedService.CollectionDate.ToUniversalTime();
                existing.DeliveryDate = updatedService.DeliveryDate.ToUniversalTime();
                existing.Description = updatedService.Description;
                existing.Phone = updatedService.Phone;
                existing.Email = updatedService.Email;

                // ✅ Reset Quote and Assigned Staff
                existing.QuotePrice = null;

                var existingAssignments = _context.ServiceStaff
                    .Where(ss => ss.ServiceModelId == existing.Id);
                _context.ServiceStaff.RemoveRange(existingAssignments);

                _context.SaveChanges();
                return RedirectToAction("Portal");
            }

            return View(updatedService);
        }

        [Authorize]
        public IActionResult DeleteBooking(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var service = _context.Services
                .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (service == null)
                return NotFound();

            return View(service); // Renders DeleteBooking.cshtml
        }


        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteBookingConfirmed(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var service = _context.Services
                .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (service == null)
                return NotFound();

            // Remove assigned staff
            var assignments = _context.ServiceStaff.Where(ss => ss.ServiceModelId == id);
            _context.ServiceStaff.RemoveRange(assignments);

            _context.Services.Remove(service);
            _context.SaveChanges();

            return RedirectToAction("Portal");
        }





    }
}
