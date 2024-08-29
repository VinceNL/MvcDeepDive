using Microsoft.EntityFrameworkCore;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(MvcShopContext context)
            : base(context)
        {
        }

        public Cart CreateOrUpdate(Guid? cartId, Guid productId, int quantity = 1)
        {
            (var cart, var isNewCart) = GetCart(cartId);

            AddProductToCart(cart, productId, quantity);

            if (isNewCart)
            {
                _context.Add(cart);
            }
            else
            {
                _context.Update(cart);
            }

            return cart;
        }

        private void AddProductToCart(Cart cart, Guid productId, int quantity)
        {
            var lineItem = cart.LineItems.FirstOrDefault(x => x.ProductId == productId);

            if (lineItem is not null && quantity == 0)
            {
                cart.LineItems.Remove(lineItem);
            }
            else if (lineItem is not null)
            {
                lineItem.Quantity = quantity;
            }
            else
            {
                lineItem = new() { ProductId = productId, Quantity = quantity };
                _context.LineItems.Add(lineItem);
                cart.LineItems.Add(lineItem);
            }
        }

        private (Cart cart, bool isNewCart) GetCart(Guid? cartId)
        {
            Cart? cart = null;
            bool isNewCart = false;

            if (cartId is not null || cartId == Guid.Empty)
            {
                cart = _context.Carts
                    .Include(x => x.LineItems)
                    .FirstOrDefault(x => x.CartId == cartId);
            }

            if (cart is null)
            {
                isNewCart = true;
                cart = new();
            }

            return (cart, isNewCart);
        }
    }
}