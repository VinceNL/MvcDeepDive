using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository(MvcShopContext context) : base(context)
        {
        }
    }
}