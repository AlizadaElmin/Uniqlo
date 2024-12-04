using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.DataAccess;

public class UniqloDbContext:IdentityDbContext<User>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    public UniqloDbContext(DbContextOptions options) : base(options)
    {
    }
    
}