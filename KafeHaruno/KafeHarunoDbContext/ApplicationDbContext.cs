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
            modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId }); // Birleşik anahtar tanımı


            //bir tablenın birden fazla orderı olabilir 
            modelBuilder.Entity<Tables>()
                  .HasMany(a => a.Orders)
                  .WithOne(a => a.Tables)
                  .HasForeignKey(a => a.TableId)
                  .IsRequired();

            // table bir den fazla bill olabilir ama bir bill bir masanınındır
            modelBuilder.Entity<Tables>()
                 .HasMany(a=>a.Bills) // Bir masanın birden fazla faturası olabilir
                 .WithOne(b => b.Tables) // Bir fatura sadece bir masaya ait olabilir
                 .HasForeignKey(b => b.TableId); // Yabancı anahtar bağlantısı

            // her bir orderın yalnız bir orderı olabilir ama bir userın birden fazla orderı olucaktır 
            modelBuilder.Entity<User>()
                .HasMany(a => a.Orders)
                .WithOne(a=>a.User)
                .HasForeignKey(a => a.UserId)
                .IsRequired();

            // Apartment ile ApartmentType ilişkisi (Güncellenmiş)
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
               .HasPrecision(10, 2); // 10 basamaklı, 2 ondalık basamak
        }

    }
}
