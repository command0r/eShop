using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    public Task<Order> CreateOderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress)
    {
        throw new System.NotImplementedException();
    }

    public Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
    {
        throw new System.NotImplementedException();
    }

    public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
        throw new System.NotImplementedException();
    }

    public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
        throw new System.NotImplementedException();
    }
}