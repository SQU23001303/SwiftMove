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
            var latestReviews = _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .ToList();

            return View(latestReviews);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            // (Optional: Do something with the message like logging, emailing, etc.)

            TempData["SuccessMessage"] = "Thank you, your message has been submitted!";
            return RedirectToAction("Contact");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Create Booking
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

                    TempData["SuccessMessage"] = "Booking submitted successfully!";

                    return RedirectToAction("Bookings");
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

            var reviews = _context.Reviews
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Loyalty calculations
            decimal totalSpent = bookings
                .Where(b => b.QuotePrice.HasValue)
                .Sum(b => b.QuotePrice.Value);

            string loyaltyLevel = totalSpent switch
            {
                >= 1000 => "Gold",
                >= 500 => "Silver",
                _ => "Bronze"
            };

            var bookedMonths = bookings
                .Select(b => b.CollectionDate.ToString("yyyy-MM"))
                .Distinct()
                .Count();

            bool freeMonthUnlocked = bookedMonths >= 11;

            ViewBag.LoyaltyLevel = loyaltyLevel;
            ViewBag.TotalSpent = totalSpent;
            ViewBag.FreeMonthUnlocked = freeMonthUnlocked;
            ViewBag.MonthsBooked = bookedMonths;

            var model = new PortalViewModel
            {
                Bookings = bookings,
                Reviews = reviews
            };

            return View(model);
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
                existing.ServiceListId = updatedService.ServiceListId;
                existing.CollectionDate = updatedService.CollectionDate.ToUniversalTime();
                existing.DeliveryDate = updatedService.DeliveryDate.ToUniversalTime();
                existing.Description = updatedService.Description;
                existing.Phone = updatedService.Phone;
                existing.Email = updatedService.Email;

                // Reset Quote and Assigned Staff
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


        [Authorize]
        public IActionResult CreateReview()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult CreateReview(Review review)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity.Name;

            if (ModelState.IsValid)
            {
                review.UserId = userId;
                review.UserName = userName;
                review.CreatedAt = DateTime.UtcNow;

                _context.Reviews.Add(review);
                _context.SaveChanges();

                return RedirectToAction("Portal");
            }

            return View(review);
        }

        [Authorize]
        public IActionResult EditReview(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null) return NotFound();

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult EditReview(Review updated)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var review = _context.Reviews.FirstOrDefault(r => r.Id == updated.Id && r.UserId == userId);
            if (review == null) return NotFound();

            if (ModelState.IsValid)
            {
                review.Rating = updated.Rating;
                review.Comment = updated.Comment;
                _context.SaveChanges();
                return RedirectToAction("Portal");
            }

            return View(updated);
        }

        [Authorize]
        public IActionResult DeleteReview(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null) return NotFound();

            return View(review); // This should go to DeleteReview.cshtml
        }


        [HttpPost, ActionName("DeleteReview")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeleteReviewConfirmed(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null) return NotFound();

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return RedirectToAction("Portal");
        }



    }
}
