using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;
using Product = Core.Entities.Product;


namespace Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
        _config = config;
    }

    public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
    {
        StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
        var basket = await _basketRepository.GetBasketAsync(basketId);
        
        // Check if we have a basket before creating a payment intent
        if (basket == null) return null;
        
        var shippingPrice = 0m;
        
        // Check to see if we have a delivery method in a basket
        if (basket.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync((int)basket.DeliveryMethodId);
            shippingPrice = deliveryMethod.Price;
        }
        
        // Check basket to make sure the prices there are accurate (server-side validation)
        foreach (var item in basket.Items)
        {
            var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

        // Creating payment intent
        var service = new PaymentIntentService();
        PaymentIntent intent;
        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long) basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)
                    shippingPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = new List<string> {"card"}
            };
            intent = await service.CreateAsync(options);
            basket.PaymentIntentId = intent.Id;
            basket.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long) basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)
                    shippingPrice * 100
            };
            await service.UpdateAsync(basket.PaymentIntentId, options);
        }
        // Update basket with the current state (correct prices and payment intent)
        await _basketRepository.UpdateBasketAsync(basket);
        return basket;
    }

    // Methods that process Stripe events
    public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
    {
        var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null) return null;

        order.Status = OrderStatus.PaymentReceived;
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.Complete();
        return order;
    }

    public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
    {
        var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        
        if (order == null) return null;
        order.Status = OrderStatus.PaymentFailed;
        await _unitOfWork.Complete();
        return order;
    }
}