using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Models;
using Uniqlo.ViewModels.Commons;
using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Sliders;


namespace Uniqlo.Controllers;

public class HomeController(UniqloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        HomeVM vm = new();
        vm.Sliders = await _context.Sliders.Select(x => new SliderListItemVM()
        {
            ImageUrl = x.ImageUrl,
            Link = x.Link,
            Subtitle = x.Subtitle,
            Title = x.Title
        }).ToListAsync();
        vm.Products = await _context.Products.Select(x=>new ProductListItemVM()
        {
            CoverImage = x.CoverImage,
            Discount = x.Discount,
            Id = x.Id,
            Name = x.Name,
            IsInStock = x.Quantity > 0,
            SellPrice = x.SellPrice
            
        }).ToListAsync();
        return View(vm);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
}