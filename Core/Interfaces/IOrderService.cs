using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);
    Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);
    Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
    Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
}