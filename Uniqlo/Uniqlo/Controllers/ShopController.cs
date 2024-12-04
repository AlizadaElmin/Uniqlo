using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
using Uniqlo.Models;
using Uniqlo.ViewModels.Baskets;
using Uniqlo.ViewModels.Brands;
using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Shops;

namespace Uniqlo.Controllers;

public class ShopController(UniqloDbContext _context):Controller
{
    public async Task<IActionResult> Index(int? catId,string amount)
    {
        var query = _context.Products.AsQueryable();
        if (catId.HasValue)
        {
            query = query.Where(x=>x.BrandId == catId);
        }

        if (amount != null)
        {
            var prices= amount.Split('-').Select(x=>Convert.ToInt32(x));
            query = query
                .Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
        }
        ShopVM vM = new ShopVM();
        vM.Brands = await _context.Brands
            .Where(x => !x.IsDeleted)
            .Select(x => new BrandAndProductVM()
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products.Count
            })
            .ToListAsync();

        vM.Products = await query
            .Take(6)
            .Select(x => new ProductListItemVM()
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                Name = x.Name,
                IsInStock = x.Quantity > 0,
                SellPrice = x.SellPrice
            }).ToListAsync();
        vM.ProductCount = await query.CountAsync(); 
        return View(vM);
    }
    
    public async Task<IActionResult> AddBasket(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            item.Count++;
        }
        else
        {
            basket.Add(new BasketCookieItemVM
            {
                Id = id,
                Count = 1
            });
        }
        string data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", data);
        return Ok();
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Products.Include(x=>x.Images).Include(x=>x.ProductRatings).Where(x=>x.Id == id.Value && !x.IsDeleted).FirstOrDefaultAsync();
        if (data is null) return NotFound();
        string? userId = User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier).Value;
        if (userId is not null)
        {
           var rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id.Value)
                .Select(x => x.RatingRate).FirstOrDefaultAsync();
           ViewBag.Rating = rating == 0 ? 5 : rating;
        }
        else
        {
            ViewBag.Rating = 5;
        }
        
        
        return View(data);
    }

    [Authorize]
    public async Task<IActionResult> Rate(int? productId, int rate = 1)
    {
        if (!productId.HasValue) return BadRequest();
        string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        if (!await _context.Products.AnyAsync(p => p.Id == productId)) return NotFound();
        var rating = await _context.ProductRatings.Where(x=>x.ProductId == productId && x.UserId == userId).FirstOrDefaultAsync();
        if (rating is null)
        {
            await _context.ProductRatings.AddAsync(new ProductRating()
            {
                ProductId = productId,
                RatingRate = rate,
                UserId = userId
            });
        }
        else
        {
            rating.RatingRate = rate;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = productId });
    }
    
    
    
    
    public async Task<IActionResult> GetBasket(int id)
    {

        return Json(getBasket());
    }

    public async Task<IActionResult> RemoveBasket(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
     
        basket.Remove(item);
        string data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", data);
        return RedirectToAction(nameof(Index));
    }
    
    
    List<BasketCookieItemVM> getBasket()
    {
        try
        {
            string? value = HttpContext.Request.Cookies["basket"];
            if (value is null)
            {
                return new();
            }
            return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? new();
        }
        catch
        {
            return new();
        }
    }

}