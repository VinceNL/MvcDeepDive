using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository(MvcShopContext context) : base(context)
        {
        }
    }
}