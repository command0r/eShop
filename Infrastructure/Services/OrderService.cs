using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IBasketRepository _basketRepo;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
    {
        _basketRepo = basketRepo;
        _unitOfWork = unitOfWork;
    } 
    
    public async Task<Order> CreateOderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
    {
        // Get basket from the repo (check the item price from a db)
        // Get delivery method, calc subtotal, create order and save to a db 
        var basket = await _basketRepo.GetBasketAsync(basketId);
        var items = new List<OrderItem>();
        foreach (var item in basket.Items)
        {
            var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
            var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
            var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
            items.Add(orderItem);
        }

        var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
        var subtotal = items.Sum(item => item.Price * item.Quantity);
        var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
        
        // Save to a Db
        _unitOfWork.Repository<Order>().Add(order);
        var result = await _unitOfWork.Complete();

        // Let the controller to send the error back in case of the failure
        if (result <= 0) return null;
        
        // Delete basket after successful save operation to a database
        await _basketRepo.DeleteBasketAsync(basketId);

        return order;
    }

    public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
    {
        var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
        return await _unitOfWork.Repository<Order>().ListAsync(spec);
    }

    public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
    {
        var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
        return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
    }

    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
        return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
    }
}