using MvcShop.Domain.Models;

namespace MvcShop.Web.Models
{
    public class ShoppingCartModel
    {
        public Cart? Cart { get; set; }
        public bool IsCompact { get; set; }
    }
}
