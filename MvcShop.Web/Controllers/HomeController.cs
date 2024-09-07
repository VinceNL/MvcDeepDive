using Microsoft.AspNetCore.Mvc;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Attributes;
using MvcShop.Web.Filters;
using MvcShop.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MvcShop.Web.Controllers
{
    public class HomeController(IRepository<Product> productRepository, ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IRepository<Product> _productRepository = productRepository;

        [TimerFilter]
        //[ServiceFilter(typeof(TimerFilter))]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/details/{productId:guid}/{slug:slugTransform}")]
        public IActionResult TicketDetails(Guid productId, [RegularExpression("^[a-zA-Z0-9- ]+$")] string slug)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }

            var product = _productRepository.Get(productId);

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
