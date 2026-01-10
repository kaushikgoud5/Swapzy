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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

        }
    }

}
