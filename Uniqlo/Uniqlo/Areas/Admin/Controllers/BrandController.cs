using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Models;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class BrandController(IWebHostEnvironment _env,UniqloDbContext _context):Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Brands.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Brand newBrand)
    {
        Brand brand = new Brand()
        {
            Name = newBrand.Name,
        };
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Brand brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        return View(brand);
    }
    
    [HttpPost]
    public async Task<ActionResult> Update(int id,Brand br)
    {
        Brand brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        brand.Name = br.Name;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        Brand brand = await _context.Brands.FindAsync(id);
        if (brand == null) {return NotFound();}
        brand.IsDeleted = true;
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}