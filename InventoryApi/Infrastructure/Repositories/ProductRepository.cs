using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(InventoryDbContext ctx) : base(ctx) { }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredAsync(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            int page,
            int limit,
            string? search)
        {
            var q = _ctx.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                q = q.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                q = q.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                q = q.Where(p => p.Price <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.Name.Contains(search) || (p.Description ?? "").Contains(search));

            var total = await q.CountAsync();
            var items = await q
                .OrderBy(p => p.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (items, total);
        }
    }
}
