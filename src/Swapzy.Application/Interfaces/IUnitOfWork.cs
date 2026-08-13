namespace Swapzy.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IInterestRepository Interests { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    }
}
