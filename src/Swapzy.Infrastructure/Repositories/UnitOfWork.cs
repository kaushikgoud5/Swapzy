using Microsoft.EntityFrameworkCore.Storage;
using Swapzy.Application.Interfaces;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SwapzyDbContext _context;
        private IDbContextTransaction _transaction;

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(
            SwapzyDbContext context,
            IUserRepository users,
            IRoleRepository roles,
            ICategoryRepository categories,
            IProductRepository products)
        {
            _context = context;
            Users = users;
            Roles = roles;
            Categories = categories;
            Products = products;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
