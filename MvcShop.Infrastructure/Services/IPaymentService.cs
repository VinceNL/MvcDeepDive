using MvcShop.Infrastructure.Enums;

namespace MvcShop.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<PaymentStatus> GetStatusAsync(Guid orderId);
    }
}