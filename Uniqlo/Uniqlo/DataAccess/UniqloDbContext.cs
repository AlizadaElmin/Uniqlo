using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.DataAccess;

public class UniqloDbContext:DbContext       
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    
    public UniqloDbContext(DbContextOptions options) : base(options)
    {
    }
    
}