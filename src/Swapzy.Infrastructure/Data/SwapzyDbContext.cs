using Microsoft.EntityFrameworkCore;
using Swapzy.Core.Entities.Authorization;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Infrastructure.Data
{
    public class SwapzyDbContext : DbContext
    {
        public SwapzyDbContext(DbContextOptions<SwapzyDbContext> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(x => new { x.RoleId, x.PermissionId });

            modelBuilder.Entity<RoleScope>()
                .HasKey(x => new { x.RoleId, x.ScopeId });

            modelBuilder.Entity<Permission>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Scope>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }

    }

}
