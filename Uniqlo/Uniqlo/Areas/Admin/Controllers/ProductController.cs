using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Extensions;
using Uniqlo.Models;
using Uniqlo.ViewModels.Products;

namespace Uniqlo.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(IWebHostEnvironment _env,UniqloDbContext _context):Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Brands.Where(x=>!x.IsDeleted).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File must be an image");
            }

            if (!vm.File.IsValidSize(400))
            {
                ModelState.AddModelError("File", "File size must be less than 400 KB");
            }
        }

        if (!ModelState.IsValid) return View(vm);
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand does not exist");
            return View();
        }
        Product product = vm;
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath,"imgs","products");
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        Product product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public async Task<ActionResult> Update(int id,ProductCreateVM vm)
    {
        Product product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
            {
                ModelState.AddModelError("File", "File must be an image");
            }

            if (!vm.File.IsValidSize(400))
            {
                ModelState.AddModelError("File", "File size must be less than 400 KB");
            }
        }

        if (!ModelState.IsValid) return View(vm);
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand does not exist");
            return View(vm);
        }
        product.Name = vm.Name;
        product.Description = vm.Description;
        product.CostPrice = vm.CostPrice;
        product.SellPrice = vm.SellPrice;
        product.Discount = vm.Discount;
        product.BrandId = vm.BrandId;
        product.Quantity = vm.Quantity;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        Product product = await _context.Products.FindAsync(id);
        if (product == null) {return NotFound();}
        string filePath =Path.Combine(_env.WebRootPath,"imgs","products",product.CoverImage);
        if(System.IO.File.Exists(filePath)) {System.IO.File.Delete(filePath);}
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}