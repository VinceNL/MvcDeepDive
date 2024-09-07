using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcShop.Domain.Models;
using MvcShop.Web.Tests.Repositories;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Controllers;
using MvcShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvcShop.Infrastructure.Data;

namespace MvcShop.Web.Tests;

[TestClass]
public class CartControllerTests
{
    private MvcShopContext context = default!;
    private ILogger<CartController> logger = default!;

    [TestInitialize]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(options => options.AddDebug())
            .BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

        logger = factory.CreateLogger<CartController>();

        context = new MvcShopContext();

        context.Database.EnsureDeleted();

        context.Database.Migrate();

        context.Products.Add(new Product { ProductId = Guid.Parse("4bc34cb4-c16e-4172-97af-4f90d2c494ec"), Name = "Alexander Lemtov Live", Price = 65m });
        context.Products.Add(new Product { ProductId = Guid.Parse("cda496ae-ec4d-410f-8bcd-26aaca5ba9da"), Name = "To The Moon And Back", Price = 135m });
        context.Products.Add(new Product { ProductId = Guid.Parse("92bc5f1c-0851-4fbb-931a-d6f807aae99a"), Name = "The State Of Affairs: Mariam Live!", Price = 85m });

        context.SaveChanges();
    }

    [TestMethod]
    public void Cart_Update_With_Empty_CartId_Should_Create_New_Cart()
    {
        var cartController = new CartController(
            logger,
            new CartRepository(context),
            new CustomerRepository(context),
            new OrderRepository(context),
            new InMemoryStateRepository());

        var model = new AddToCartModel
        {
            CartId = null,
            Product = new ProductModel
            {
                ProductId = Guid.Parse("4bc34cb4-c16e-4172-97af-4f90d2c494ec"),
                Quantity = 99
            }
        };

        var result = cartController.AddToCart(model);

        Assert.AreEqual(99, context.Carts.Sum(x => x.LineItems.Sum(x => x.Quantity)));

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

    [TestMethod]
    public void Cart_Update_With_Existing_CartId_Should_Update_Cart()
    {
        // Arrange: Create a new cart and add an initial product
        var initialCart = new Cart
        {
            CartId = Guid.NewGuid(),
            LineItems = new List<LineItem>
        {
            new LineItem
            {
                ProductId = Guid.Parse("4bc34cb4-c16e-4172-97af-4f90d2c494ec"),
                Quantity = 1
            }
        }
        };
        context.Carts.Add(initialCart);
        context.SaveChanges();

        var cartController = new CartController(
            logger,
            new CartRepository(context),
            new CustomerRepository(context),
            new OrderRepository(context),
            new InMemoryStateRepository());

        var model = new AddToCartModel
        {
            CartId = initialCart.CartId,
            Product = new ProductModel
            {
                ProductId = Guid.Parse("cda496ae-ec4d-410f-8bcd-26aaca5ba9da"),
                Quantity = 2
            }
        };

        // Act: Add another product to the existing cart
        var result = cartController.AddToCart(model);

        // Assert: Verify that the cart is updated correctly
        var updatedCart = context.Carts.Include(c => c.LineItems).FirstOrDefault(c => c.CartId == initialCart.CartId);
        Assert.IsNotNull(updatedCart);
        Assert.AreEqual(2, updatedCart.LineItems.Count);
        Assert.AreEqual(1, updatedCart.LineItems.First(li => li.ProductId == Guid.Parse("4bc34cb4-c16e-4172-97af-4f90d2c494ec")).Quantity);
        Assert.AreEqual(2, updatedCart.LineItems.First(li => li.ProductId == Guid.Parse("cda496ae-ec4d-410f-8bcd-26aaca5ba9da")).Quantity);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
    }

}