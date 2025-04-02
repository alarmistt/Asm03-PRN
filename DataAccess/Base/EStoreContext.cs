using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Base
{
    public class EStoreContext : DbContext
    {

        public EStoreContext(DbContextOptions<EStoreContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
