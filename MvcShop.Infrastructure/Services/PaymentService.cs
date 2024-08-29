using Microsoft.Extensions.Logging;
using MvcShop.Infrastructure.Enums;

namespace MvcShop.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<IPaymentService> _logger;

        public PaymentService(ILogger<IPaymentService> logger)
        {
            _logger = logger;
        }

        public Task<PaymentStatus> GetStatusAsync(Guid orderId)
        {
            _logger.LogInformation($"Retrieving payment status for {orderId}");

            return Task.FromResult(PaymentStatus.Processing);
        }
    }
}