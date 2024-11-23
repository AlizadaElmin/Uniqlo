using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Models;


namespace Uniqlo.Controllers;

public class HomeController(UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Sliders.ToListAsync());
    }

    public IActionResult About()
    {
        Product product = new Product();
   
        
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
}