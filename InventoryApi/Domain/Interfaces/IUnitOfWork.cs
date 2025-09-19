using InventoryApi.Infrastructure.Data.Models;

namespace InventoryApi.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
