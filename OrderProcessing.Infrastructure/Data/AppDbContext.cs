using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Entity;

namespace OrderProcessing.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Order> Orders { get; set; }
}