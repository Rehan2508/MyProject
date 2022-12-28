using Microsoft.EntityFrameworkCore;

namespace CateringWebApplication.Models
{
    public class CateringContext : DbContext
    {
        public CateringContext(DbContextOptions<CateringContext> option) : base(option)
        {

        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }   
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Sale> sales { get; set; }
        public DbSet<ProductSold> productsSold { get; set; }
    }
}
