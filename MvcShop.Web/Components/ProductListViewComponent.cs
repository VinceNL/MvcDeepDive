using Microsoft.AspNetCore.Mvc;
using MvcShop.Domain.Models;
using MvcShop.Infrastructure.Repositories;

namespace MvcShop.Web.Components
{
    public class ProductListViewComponent(IRepository<Product> repository, ILogger<ProductListViewComponent> logger) : ViewComponent
    {
        private readonly IRepository<Product> _repository = repository;
        private readonly ILogger<ProductListViewComponent> _logger = logger;

        public Task<IViewComponentResult> InvokeAsync()
        {
            var products = _repository.GetAll();

            _logger.LogInformation($"Found a total of {products.Count()} products");

            return Task.FromResult<IViewComponentResult>(View(products));
        }
    }
}