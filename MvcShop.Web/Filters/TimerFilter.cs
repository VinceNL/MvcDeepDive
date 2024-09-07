using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MvcShop.Web.Filters
{
    public class TimerFilter(ILogger<TimerFilter> logger) : IAsyncActionFilter
    {
        private readonly ILogger<TimerFilter> _logger = logger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _logger.LogInformation($"Action {context.ActionDescriptor} started");

            await next();

            stopwatch.Stop();

            _logger.LogInformation($"Action {context.ActionDescriptor} finished in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}