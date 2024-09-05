using Microsoft.AspNetCore.Mvc;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Models;

namespace MvcShop.Web.Controllers
{
    [Route("[controller]")]
    public class CartController(
        ILogger<CartController> logger,
        ICartRepository cartRepository,
        IRepository<Customer> customerRepository,
        IRepository<Order> orderRepository) : Controller
    {
        private readonly ILogger<CartController> _logger = logger;
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly IRepository<Customer> _customerRepository = customerRepository;
        private readonly IRepository<Order> _orderRepository = orderRepository;
        //private readonly IStateRepository _stateRepository = stateRepository;

        public IActionResult Index(Guid? id)
        {
            return View();
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult AddToCart(AddToCartModel addToCartModel)
        {
            if (addToCartModel.Product is null)
            {
                return BadRequest();
            }

            _logger.LogInformation($"Adding products " +
                $"{addToCartModel.Product.ProductId} to cart " +
                $"{addToCartModel.CartId}");

            var cart = _cartRepository.CreateOrUpdate(addToCartModel.CartId,
                addToCartModel.Product.ProductId,
                addToCartModel.Product.Quantity);

            _cartRepository.SaveChanges();

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [Route("Update")]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UpdateQuantitiesModel updateQuantitiesModel)
        {
            if (updateQuantitiesModel.Products is null)
            {
                return BadRequest();
            }

            Cart cart = null!;

            foreach (var product in updateQuantitiesModel.Products)
            {
                logger.LogInformation($"Adding products {product.ProductId} to cart {updateQuantitiesModel.CartId}");

                cart = cartRepository.CreateOrUpdate(updateQuantitiesModel.CartId,
                    product.ProductId, product.Quantity);
            }

            _cartRepository.SaveChanges();

            return RedirectToAction("Index", "Cart");
     
        }

        [HttpPost]
        [Route("Finalize")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateOrderModel createOrderModel)
        {
            if (createOrderModel.Customer is null)
            {
                ModelState.AddModelError("Customer",
                    "Customer data is not set correctly");

                return View("Index");
            }

            if (createOrderModel.Customer.Name.Length <= 2)
            {
                ModelState.AddModelError(nameof(createOrderModel.Customer.Name),
                    "Name is too short");

                return View("Index");
            }

            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var customer = new Customer
            {
                Email = createOrderModel.Customer.Email,
                Name = createOrderModel.Customer.Name,
                City = createOrderModel.Customer.City,
                Country = createOrderModel.Customer.Country,
                ShippingAddress = createOrderModel.Customer.ShippingAddress,
                PostalCode = createOrderModel.Customer.PostalCode,
            };

            _logger.LogInformation($"Creating new order for {customer.CustomerId}");

            _customerRepository.Add(customer);

            var order = new Order
            {
                CustomerId = customer.CustomerId
            };

            if (createOrderModel.CartId is null || createOrderModel.CartId == Guid.Empty)
            {
                ModelState.AddModelError("Cart", "Cart has been deleted");

                return View("Index");
            }

            var cart = _cartRepository.Get(createOrderModel.CartId.Value);

            if (cart is null)
            {
                ModelState.AddModelError("Cart", "Cart has been deleted");

                return View("Index");
            }

            foreach (var lineItem in cart.LineItems)
            {
                order.LineItems.Add(lineItem);
            }

            _orderRepository.Add(order);
            _cartRepository.Update(cart);
            _cartRepository.SaveChanges();

            _logger.LogInformation($"Order placed for {customer.CustomerId}");

            //_stateRepository.Remove("NumberOfItems");
            //_stateRepository.Remove("CartId");

            return RedirectToAction("ThankYou");
        }

        [Route("ThankYou")]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}