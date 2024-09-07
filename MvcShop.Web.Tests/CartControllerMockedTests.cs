using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using MvcShop.Web.Controllers;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Domain.Models;
using MvcShop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MvcShop.Web.Tests
{
    [Collection("CartControllerMockedTests")]
    public class CartControllerMockedTests
    {
        private readonly Mock<ILogger<CartController>> _mockLogger;
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IRepository<Customer>> _mockCustomerRepository;
        private readonly Mock<IRepository<Order>> _mockOrderRepository;
        private readonly Mock<IStateRepository> _mockStateRepository;
        private readonly CartController _controller;

        public CartControllerMockedTests()
        {
            _mockLogger = new Mock<ILogger<CartController>>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockCustomerRepository = new Mock<IRepository<Customer>>();
            _mockOrderRepository = new Mock<IRepository<Order>>();
            _mockStateRepository = new Mock<IStateRepository>();

            _controller = new CartController(
                _mockLogger.Object,
                _mockCartRepository.Object,
                _mockCustomerRepository.Object,
                _mockOrderRepository.Object,
                _mockStateRepository.Object
            );
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddToCart_ProductIsNull_ReturnsBadRequest()
        {
            // Arrange
            var model = new AddToCartModel { Product = null };

            // Act
            var result = _controller.AddToCart(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void AddToCart_ValidProduct_RedirectsToIndex()
        {
            // Arrange
            var model = new AddToCartModel
            {
                CartId = Guid.NewGuid(),
                Product = new ProductModel { ProductId = Guid.NewGuid(), Quantity = 1 }
            };
            var cart = new Cart { CartId = (Guid)model.CartId, LineItems = new List<LineItem> { new LineItem { ProductId = Guid.Parse("92bc5f1c-0851-4fbb-931a-d6f807aae99a"), Quantity = 1 } } };

            _mockCartRepository.Setup(repo => repo.CreateOrUpdate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>())).Returns(cart);

            // Act
            var result = _controller.AddToCart(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Cart", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void Update_ProductsIsNull_ReturnsBadRequest()
        {
            // Arrange
            var model = new UpdateQuantitiesModel { Products = null! };

            // Act
            var result = _controller.Update(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Update_ValidProducts_RedirectsToIndex()
        {
            // Arrange
            var model = new UpdateQuantitiesModel
            {
                CartId = Guid.NewGuid(),
                Products = new List<ProductModel> { new ProductModel { ProductId = Guid.Parse("92bc5f1c-0851-4fbb-931a-d6f807aae99a"), Quantity = 1 } }
            };
            var cart = new Cart { CartId = (Guid)model.CartId, LineItems = new List<LineItem> { new LineItem { ProductId = Guid.Parse("92bc5f1c-0851-4fbb-931a-d6f807aae99a"), Quantity = 1 } } };

            _mockCartRepository.Setup(repo => repo.CreateOrUpdate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>())).Returns(cart);

            // Act
            var result = _controller.Update(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Cart", redirectToActionResult.ControllerName);
        }

        [Fact]
        public void Create_CustomerIsNull_ReturnsViewWithModelError()
        {
            // Arrange
            var model = new CreateOrderModel { Customer = null! };

            // Act
            var result = _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void ThankYou_ReturnsViewResult()
        {
            // Act
            var result = _controller.ThankYou();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
