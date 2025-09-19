using InventoryApi.Infrastructure.Data.Models;

namespace InventoryApi.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
