using Microsoft.EntityFrameworkCore;
using Swapzy.Core.Entities.Authorization;
using Swapzy.Core.Entities.Categories;
using Swapzy.Core.Entities.Interests;
using Swapzy.Core.Entities.Notifications;
using Swapzy.Core.Entities.Products;
using Swapzy.Core.Entities.Users;
using Message = Swapzy.Core.Entities.Interests.Message;

namespace Swapzy.Infrastructure.Data
{
    public class SwapzyDbContext : DbContext
    {
        public SwapzyDbContext(DbContextOptions<SwapzyDbContext> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserPreferredCategory> UserPreferredCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMetadata> ProductMetadata { get; set; }
        public DbSet<ProductLocation> ProductLocations { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email).IsUnique();

                entity.HasOne(x => x.Profile)
                      .WithOne(x => x.User)
                      .HasForeignKey<UserProfile>(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(x => x.UserId);
            });
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                    .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Slug).IsUnique();
                entity.HasIndex(x => x.IsActive);
            });

            modelBuilder.Entity<UserPreferredCategory>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => new { x.UserId, x.CategoryId })
                      .IsUnique();

                entity.HasOne(x => x.User)
                      .WithMany(x => x.PreferredCategories)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Category)
                      .WithMany(x => x.UserPreferredCategories)
                      .HasForeignKey(x => x.CategoryId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.OwnerId);
                entity.HasIndex(x => x.ProductCategoryId);
                entity.HasIndex(x => x.Status);
                entity.HasIndex(x => x.IsAvailable);
                entity.HasIndex(x => x.CreatedOn);

                entity.HasOne(x => x.Owner)
                      .WithMany(x => x.Products)
                      .HasForeignKey(x => x.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Category)
                      .WithMany(x => x.Products)
                      .HasForeignKey(x => x.ProductCategoryId);
            });


            modelBuilder.Entity<ProductMetadata>(entity =>
            {
                entity.HasKey(x => x.ProductId);

                entity.HasOne(x => x.Product)
                      .WithOne(x => x.Metadata)
                      .HasForeignKey<ProductMetadata>(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductLocation>(entity =>
            {
                entity.HasKey(x => x.ProductId);

                entity.HasOne(x => x.Product)
                      .WithOne(x => x.Location)
                      .HasForeignKey<ProductLocation>(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.City);
                entity.HasIndex(x => new { x.Country, x.City });

                entity.Property(x => x.GeoLocation)
                      .HasColumnType("geography(Point, 4326)");

                entity.HasIndex(x => x.GeoLocation)
                      .HasMethod("GIST");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.ProductId);

                entity.HasOne(x => x.Product)
                      .WithMany(x => x.Images)
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.UserId);
                entity.HasIndex(x => x.IsRead);
            });

            modelBuilder.Entity<Interest>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => new { x.BuyerId, x.ProductId }).IsUnique();
                entity.HasIndex(x => new { x.SellerId, x.Status });
                entity.HasIndex(x => new { x.BuyerId, x.Status });

                entity.HasOne(x => x.Buyer)
                      .WithMany()
                      .HasForeignKey(x => x.BuyerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Seller)
                      .WithMany()
                      .HasForeignKey(x => x.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Product)
                      .WithMany()
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.InterestId).IsUnique();
                entity.HasIndex(x => x.BuyerId);
                entity.HasIndex(x => x.SellerId);

                entity.HasOne(x => x.Interest)
                      .WithMany()
                      .HasForeignKey(x => x.InterestId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Buyer)
                      .WithMany()
                      .HasForeignKey(x => x.BuyerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Seller)
                      .WithMany()
                      .HasForeignKey(x => x.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Product)
                      .WithMany()
                      .HasForeignKey(x => x.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => new { x.MatchId, x.CreatedOn });

                entity.HasOne(x => x.Match)
                      .WithMany()
                      .HasForeignKey(x => x.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Sender)
                      .WithMany()
                      .HasForeignKey(x => x.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Ensure all DateTime properties are stored as UTC in PostgreSQL
            configurationBuilder.Properties<DateTime>()
                .HaveColumnType("timestamp with time zone");
        }
    }
}
