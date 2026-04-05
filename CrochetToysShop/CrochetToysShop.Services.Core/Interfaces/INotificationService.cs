namespace CrochetToysShop.Services.Core.Interfaces
{
    public interface INotificationService
    {
        Task NotifyOrderCreatedAsync(int orderId, int toyId, string customerName);
    }
}
