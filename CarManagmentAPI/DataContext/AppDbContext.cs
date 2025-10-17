// Data/ApplicationDbContext.cs
using CarManagmentAPI.DataContext.Model;
using Microsoft.EntityFrameworkCore;

namespace CarManagmentAPI.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Car> Cars => Set<Car>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Brand>(e =>
            {
                e.Property(b => b.Name).HasMaxLength(100).IsRequired();
                e.Property(b => b.Country).HasMaxLength(100);
                e.HasMany(b => b.Cars)
                 .WithOne(c => c.Brand!)
                 .HasForeignKey(c => c.BrandId)
                 .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<Car>(e =>
            {
                e.Property(c => c.Model).HasMaxLength(100).IsRequired();
                e.Property(c => c.Color).HasMaxLength(50);
                e.Property(c => c.Price).HasPrecision(18, 2).IsRequired();
                e.Property(c => c.ImagePath).HasMaxLength(400);
            });
        }
    }
}
