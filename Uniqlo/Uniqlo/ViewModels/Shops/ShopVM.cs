using Uniqlo.ViewModels.Brands;
using Uniqlo.ViewModels.Products;

namespace Uniqlo.ViewModels.Shops;

public class ShopVM
{
    public IEnumerable<BrandAndProductVM> Brands { get; set; }
    public IEnumerable<ProductListItemVM> Products { get; set; }
    public int ProductCount { get; set; }
}