namespace Uniqlo.Models;

public class ProductReview
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}