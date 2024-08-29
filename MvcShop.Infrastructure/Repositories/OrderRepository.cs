using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(MvcShopContext context) : base(context)
        {
        }
    }
}