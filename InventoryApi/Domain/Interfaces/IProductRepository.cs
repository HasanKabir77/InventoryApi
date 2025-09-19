using InventoryApi.Infrastructure.Data.Models;

namespace InventoryApi.Domain.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredAsync(
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            int page,
            int limit,
            string? search
        );
    }
}
