using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data;
using InventoryApi.Infrastructure.Data.Models;

namespace InventoryApi.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _ctx;

        public UnitOfWork(InventoryDbContext ctx)
        {
            _ctx = ctx;
            Products = new ProductRepository(ctx);
            Categories = new GenericRepository<Category>(ctx);
            Users = new GenericRepository<User>(ctx);
        }

        public IProductRepository Products { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<User> Users { get; }

        public async Task<int> SaveChangesAsync() => await _ctx.SaveChangesAsync();

        public void Dispose() => _ctx.Dispose();
    }
}
