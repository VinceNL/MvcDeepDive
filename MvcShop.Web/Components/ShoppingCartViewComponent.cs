using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcShop.Infrastructure.Data;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Models;

namespace MvcShop.Web.Components
{
    public class ShoppingCartViewComponent(
        MvcShopContext context,
        IStateRepository stateRepository
        ) : ViewComponent
    {
        private readonly MvcShopContext _context = context;
        private readonly IStateRepository _stateRepository = stateRepository;

        public async Task<IViewComponentResult> InvokeAsync(string cartId,
            bool IsCompact)
        {
            if (!Guid.TryParse(cartId, out var id))
            {
                return View(new ShoppingCartModel { IsCompact = IsCompact });
            }

            var cart = await _context.Carts
                .Include(x => x.LineItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.CartId == id);

            if (cart is not null)
            {
                _stateRepository.SetValue("NumberOfItems", cart.LineItems.Sum(x => x.Quantity).ToString());
                _stateRepository.SetValue("CartId", cart.CartId.ToString());
            }

            return View(new ShoppingCartModel { Cart = cart, IsCompact = IsCompact });
        }
    }
}