using Uniqlo.Models;

namespace Uniqlo.ViewModels.Products;

public class ProductCreateVM
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    public int  Discount { get; set; }
    public int BrandId { get; set; }
    public IFormFile File { get; set; }
    public ICollection<IFormFile>? OtherFiles { get; set; } 

    public static implicit operator Product(ProductCreateVM vm)
    {
        return new Product()
        {
            Name = vm.Name,
            Description = vm.Description,
            CostPrice = vm.CostPrice,
            SellPrice = vm.SellPrice,
            Discount = vm.Discount,
            BrandId = vm.BrandId,
            Quantity = vm.Quantity,
        };
    }
}