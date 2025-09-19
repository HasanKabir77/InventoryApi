using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly InventoryDbContext _ctx;
        public GenericRepository(InventoryDbContext ctx) => _ctx = ctx;

        public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _ctx.Set<T>().ToListAsync();

        public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);

        public void Update(T entity) => _ctx.Set<T>().Update(entity);

        public void Remove(T entity) => _ctx.Set<T>().Remove(entity);

        public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();
    }
}
