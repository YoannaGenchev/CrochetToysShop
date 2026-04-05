using CrochetToysShop.Services.Core.Interfaces;

namespace CrochetToysShop.Services.Core
{
    public class NullNotificationService : INotificationService
    {
        public Task NotifyOrderCreatedAsync(int orderId, int toyId, string customerName)
        {
            return Task.CompletedTask;
        }
    }
}
