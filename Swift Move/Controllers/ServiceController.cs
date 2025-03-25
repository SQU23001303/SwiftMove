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

   


}