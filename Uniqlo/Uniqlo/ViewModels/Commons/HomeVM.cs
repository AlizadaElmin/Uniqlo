using Uniqlo.Models;
using Uniqlo.ViewModels.Sliders;
using Uniqlo.ViewModels.Products;

namespace Uniqlo.ViewModels.Commons;

public class HomeVM
{
    public IEnumerable<SliderListItemVM> Sliders { get; set; }
    public IEnumerable<Brand> Brands { get; set; }
    public IEnumerable<ProductListItemVM> PopularProducts { get; set; }
}