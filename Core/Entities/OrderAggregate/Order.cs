using System;
using System.Collections.Generic;
using Core.Helpers;

namespace Core.Entities.OrderAggregate;

public class Order : BaseEntity
{
    public Order()
    { }
    
    public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail, Address shipToAddress, DeliveryMethod deliveryMethod, decimal subtotal)
    {
        OrderItems = orderItems;
        BuyerEmail = buyerEmail;
        ShipToAddress = shipToAddress;
        DeliveryMethod = deliveryMethod;
        Subtotal = subtotal;
    }
    
    public string BuyerEmail { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeHelper.SetKindUtc(DateTime.Now);
    public Address ShipToAddress { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public IReadOnlyList<OrderItem> OrderItems { get; set; }
    public decimal Subtotal { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string PaymentIntentId { get; set; }

    // The naming is conventional - the value will be returned by AutoMapper by default via respective Dto
    public decimal GetTotal()
    {
        return Subtotal + DeliveryMethod.Price;
    }
}