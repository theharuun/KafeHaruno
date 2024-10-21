using KafeHaruno.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KafeHaruno.KafeHarunoDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Tables> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //primary key
            modelBuilder.Entity<Bill>().HasKey(t => t.Id);
            modelBuilder.Entity<Order>().HasKey(t => t.Id);
            modelBuilder.Entity<Product>().HasKey(t => t.Id);
            modelBuilder.Entity<ProductType>().HasKey(t => t.Id);
            modelBuilder.Entity<Tables>().HasKey(t => t.Id);
            modelBuilder.Entity<User>().HasKey(t => t.Id);
            modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId }); // Composite key definition


            //a table can have more than one order
            modelBuilder.Entity<Tables>()
                  .HasMany(a => a.Orders)
                  .WithOne(a => a.Tables)
                  .HasForeignKey(a => a.TableId)
                  .IsRequired();


            // table can have more than one bill but one bill belongs to one table
            modelBuilder.Entity<Tables>()
                .HasMany(a => a.Bills) // A table can have more than one bill
                .WithOne(b => b.Tables) // An invoice can belong to only one table
                .HasForeignKey(b => b.TableId); // Foreign key connection


            // Each order can have only one order, but a user can have more than one order
            modelBuilder.Entity<User>()
                .HasMany(a => a.Orders)
                .WithOne(a=>a.User)
                .HasForeignKey(a => a.UserId)
                .IsRequired();


            // Apartment and ApartmentType relationship (Updated)
            modelBuilder.Entity<Product>()
                    .HasOne(a => a.ProductType)
                    .WithMany(at => at.Products)
                    .HasForeignKey(a => a.ProductTypeId)
                   .HasConstraintName("FK_Product_ProductType");

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            modelBuilder.Entity<Bill>()
               .Property(b => b.BillPrice)
               .HasPrecision(10, 2); // 10 digits, 2 decimal places
        }

    }
}
