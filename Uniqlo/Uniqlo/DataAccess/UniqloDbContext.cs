using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.DataAccess;

public class UniqloDbContext:DbContext       
{
    public DbSet<Slider> Sliders { get; set; }

    public UniqloDbContext(DbContextOptions options) : base(options)
    {
    }
    
}