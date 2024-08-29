using Microsoft.EntityFrameworkCore;
using MvcShop.Domain.Models;

namespace MvcShop.Infrastructure.Data
{
    public class MvcShopContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=mvcshop.db;";

            optionsBuilder.UseSqlite(connectionString);
        }

        public static void CreateInitialDatabase(MvcShopContext context)
        {
            context.Database.EnsureDeleted();

            context.Database.Migrate();

            context.Products.Add(new Product { ProductId = Guid.Parse("4bc34cb4-c16e-4172-97af-4f90d2c494ec"), Name = "Product 1", Price = 85m });
            context.Products.Add(new Product { ProductId = Guid.Parse("cda496ae-ec4d-410f-8bcd-26aaca5ba9da"), Name = "Product 2", Price = 140m });
            context.Products.Add(new Product { ProductId = Guid.Parse("92bc5f1c-0851-4fbb-931a-d6f807aae99a"), Name = "Product 3", Price = 110m });

            context.SaveChanges();
        }
    }
}