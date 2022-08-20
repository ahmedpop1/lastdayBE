using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.models
{
    public class EcommerceContext:IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public EcommerceContext(DbContextOptions options):base(options)
        {
                
        }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetials> OrderDetials { get; set; }
        public virtual DbSet<user> Users { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItems> CartItems { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetials>().HasKey(ww => new { ww.OrderId, ww.ProductId });
            modelBuilder.Entity<CartItems>().HasKey(ww => new { ww.CartId, ww.productID });
            modelBuilder.Entity<Cart>().HasIndex(u => u.username).IsUnique();
            modelBuilder.Entity<Category>().HasMany(p => p.Products)
              .WithOne(c => c.Category)
              .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Brand>().HasMany(p => p.Products)
               .WithOne(b => b.Brand)
               .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<user>()
           .HasIndex(u => u.email)
           .IsUnique();
            base.OnModelCreating(modelBuilder);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=.;initial catalog=EcommerceProject;integrated security=true;MultipleActiveResultSets=true");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
