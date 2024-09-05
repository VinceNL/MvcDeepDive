namespace MvcShop.Web.Models
{
    public class UpdateQuantitiesModel
    {
        public Guid? CartId { get; set; }
        public required IEnumerable<ProductModel> Products { get; set; }
    }
}