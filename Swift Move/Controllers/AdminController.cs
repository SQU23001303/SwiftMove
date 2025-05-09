﻿using Microsoft.AspNetCore.Authorization;
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

        //Returns an index view
        public IActionResult Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                ServiceModel = _context.Services
                .Include(s => s.ServiceList)
                .Include(s => s.ServiceStaff)
                .ThenInclude(ss => ss.Staff)
                .ToList(),

                ServiceList = _context.ServiceList.ToList()
            };

            return View(dashboardViewModel);
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

        //Assigns staff, quote or both to a job
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(ServiceModel model, List<int> SelectedStaffIds, string ActionType)
        {
            var service = _context.Services
                .Include(s => s.ServiceStaff)
                .FirstOrDefault(s => s.Id == model.Id);

            if (service == null)
                return NotFound();

            if (ActionType == "quote" || ActionType == "both")
            {
                service.QuotePrice = model.QuotePrice;
            }

            if (ActionType == "staff" || ActionType == "both")
            {
                var existingAssignments = _context.ServiceStaff.Where(ss => ss.ServiceModelId == service.Id);
                _context.ServiceStaff.RemoveRange(existingAssignments);

                foreach (var staffId in SelectedStaffIds.Take(3))
                {
                    _context.ServiceStaff.Add(new ServiceStaff
                    {
                        ServiceModelId = service.Id,
                        StaffId = staffId
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //Create/Edit/Delete services on the admin dashboard
        [HttpGet]
        public IActionResult CreateService()
        {
            return View("~/Views/Admin/CreateService.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateService(ServiceList model)
        {
            if (ModelState.IsValid)
            {
                _context.ServiceList.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/Admin/CreateService.cshtml", model);
        }

        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _context.ServiceList.Find(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditService(ServiceList model)
        {
            if (ModelState.IsValid)
            {
                _context.ServiceList.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteService(int id)
        {
            var service = _context.ServiceList.Find(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        [HttpPost, ActionName("DeleteService")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteServiceConfirmed(int id)
        {
            var service = _context.ServiceList.Find(id);
            if (service == null)
                return NotFound();

            _context.ServiceList.Remove(service);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //How the Sales Report works
        [HttpGet]
        public IActionResult SalesReport(string filter = "all")
        {
            var bookingsQuery = _context.Services
                .Include(s => s.ServiceList)
                .Include(s => s.ServiceStaff)
                .AsQueryable();

            DateTime now = DateTime.UtcNow;
            switch (filter)
            {
                case "week":
                    var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                    bookingsQuery = bookingsQuery.Where(b => b.CollectionDate >= startOfWeek);
                    break;
                case "month":
                    bookingsQuery = bookingsQuery.Where(b => b.CollectionDate.Month == now.Month && b.CollectionDate.Year == now.Year);
                    break;
            }

            var bookings = bookingsQuery.ToList();

            ViewBag.TotalSales = bookings.Sum(b => b.QuotePrice ?? 0m);
            ViewBag.TotalCount = bookings.Count;
            ViewBag.CurrentFilter = filter;

            return View(bookings);
        }

    }


}
