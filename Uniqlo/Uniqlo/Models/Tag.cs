namespace Uniqlo.Models;

public class Tag:BaseEntity
{
    public string Name { get; set; }
    public ICollection<Product>? Produts { get; set; }
}