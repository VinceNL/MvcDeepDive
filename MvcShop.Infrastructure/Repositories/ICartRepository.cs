using MvcShop.Domain.Models;

namespace MvcShop.Infrastructure.Repositories
{
    public interface ICartRepository
    {
        Cart CreateOrUpdate(Guid? cartId, Guid productId, int quantity = 1);
    }
}