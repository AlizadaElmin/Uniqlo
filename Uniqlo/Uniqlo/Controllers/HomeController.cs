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
        vm.Sliders = await _context.Sliders.Where(x=>!x.IsDeleted).Select(x => new SliderListItemVM()
        {
            ImageUrl = x.ImageUrl,
            Link = x.Link,
            Subtitle = x.Subtitle,
            Title = x.Title
        }).ToListAsync();
        vm.Brands = await _context.Brands.OrderByDescending(x => x.Products!.Count)
            .Take(4).ToListAsync();
        vm.PopularProducts = await _context.Products
            .Where(x=>vm.Brands.Select(y=>y.Id).Contains(x.BrandId!.Value))
            .Take(10)
            .Select(x=>new ProductListItemVM()
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                Name = x.Name,
                IsInStock = x.Quantity > 0,
                SellPrice = x.SellPrice,
                BranId = x.BrandId!.Value
                
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
    public void SetSession(string key, string value)
    {
        HttpContext.Session.SetString(key, value);
        HttpContext.Session.Remove(key);
    }

    public IActionResult GetSession(string key)
    {
        return Content(HttpContext.Session.GetString(key) ?? string.Empty);
    }
    public void SetCookies(string key,string value)
    {
        var opt = new CookieOptions
        {
            MaxAge = TimeSpan.FromMinutes(2)
        };
        HttpContext.Response.Cookies.Append(key, value);
    }
    public IActionResult GetCookies(string key,string value)
    {
        return Content(HttpContext.Request.Cookies[key]);
    }
    public IActionResult RemoveCookies(string key)
    {
        HttpContext.Response.Cookies.Delete(key);
        return Ok();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}