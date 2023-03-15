using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers;

public class PaymentController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    // Web-hook secret to validate Stipe intent (the value is updated from Stripe CLI if tested locally: 'stripe listen')
    private readonly string _whSecret;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IConfiguration config)
    {
        _paymentService = paymentService;
        _logger = logger;
        _whSecret = config.GetSection("StripeSettings:WhSecret").Value;
    }

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
        
        // Check the basket if not null
        if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));
        
        return basket;
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(json, 
            Request.Headers["Stripe-Signature"], _whSecret);
        PaymentIntent intent;
        Order order;

        switch (stripeEvent.Type)
        {   
            case "payment_intent.succeeded":
                intent = (PaymentIntent) stripeEvent.Data.Object;
                _logger.LogInformation("Payment succeeded: ", intent.Id);
                order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                _logger.LogInformation("Order updated to payment received: ", order.Id);
                break;
            case "payment_intent.payment_failed":
                intent = (PaymentIntent) stripeEvent.Data.Object;
                _logger.LogInformation("Payment failed: ", intent.Id);
                order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                _logger.LogInformation("Payment failed: ", order.Id);
                break;
        }

        // Return empty result to Stripe to confirm an event received (otherwise Stripe will keep trying to send it)
        return new EmptyResult();
    }
}