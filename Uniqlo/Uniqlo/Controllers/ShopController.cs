using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.DataAccess;
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
}