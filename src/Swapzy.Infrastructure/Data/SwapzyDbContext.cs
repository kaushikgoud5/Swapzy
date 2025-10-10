using Microsoft.EntityFrameworkCore;
using Swapzy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Infrastructure.Data
{
    public class SwapzyDbContext : DbContext
    {
        public SwapzyDbContext(DbContextOptions<SwapzyDbContext> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
    }
}
