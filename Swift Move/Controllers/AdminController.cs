using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using Swift_Move.Data;

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
            var services = _context.Services.ToList();

            return View(services);
        }

        public IActionResult Assign(int id)
        {
            var service = _context.ServiceModels
                .Include(s => s.AssignedStaff) // To load staff info
                .FirstOrDefault(s => s.Id == id);

            ViewBag.StaffList = new SelectList(_context.Staff.ToList(), "Id", "FullName");
            return View(service);
        }

    }
}
