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
        IRepository<Order> orderRepository,
        IStateRepository stateRepository) : Controller
    {
        private readonly IRepository<Customer> _customerRepository = customerRepository;
        private readonly IRepository<Order> _orderRepository = orderRepository;
        private readonly IStateRepository _stateRepository = stateRepository;

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

            logger.LogInformation($"Adding products " +
                $"{addToCartModel.Product.ProductId} to cart " +
                $"{addToCartModel.CartId}");

            var cart = cartRepository.CreateOrUpdate(addToCartModel.CartId,
                addToCartModel.Product.ProductId,
                addToCartModel.Product.Quantity);

            cartRepository.SaveChanges();

            _stateRepository.SetValue("NumberOfItems",
                cart.LineItems.Sum(x => x.Quantity).ToString());

            _stateRepository.SetValue("CartId",
                cart.CartId.ToString());

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

            cartRepository.SaveChanges();

            _stateRepository.SetValue("NumberOfItems",
                cart.LineItems.Sum(x => x.Quantity).ToString());

            _stateRepository.SetValue("CartId",
                cart.CartId.ToString());

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

            logger.LogInformation($"Creating new order for {customer.CustomerId}");

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

            var cart = cartRepository.Get(createOrderModel.CartId.Value);

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
            cartRepository.Update(cart);
            cartRepository.SaveChanges();

            logger.LogInformation($"Order placed for {customer.CustomerId}");

            _stateRepository.Remove("NumberOfItems");
            _stateRepository.Remove("CartId");

            return RedirectToAction("ThankYou");
        }

        [Route("ThankYou")]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}