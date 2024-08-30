using Microsoft.AspNetCore.Mvc;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Repositories;
using MvcShop.Web.Models;
using System.Diagnostics;

namespace MvcShop.Web.Controllers
{
    public class HomeController(IRepository<Product> productRepository) : Controller
    {
        private readonly IRepository<Product> _productRepository = productRepository;

        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
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
