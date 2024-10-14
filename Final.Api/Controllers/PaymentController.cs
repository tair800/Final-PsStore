using Final.Api.Stripe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly StripeSettings _stripeSettings;

    // Constructor to initialize Stripe API Key
    public PaymentController(IOptions<StripeSettings> stripeOptions)
    {
        _stripeSettings = stripeOptions.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>()
        };

        // Add items to the session
        foreach (var item in request.Items)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = item.Price, // Price in the smallest currency unit
                    Currency = "usd", // Currency can be dynamic (USD, AZN, etc.)
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Title, // Product Name
                    },
                },
                Quantity = item.Quantity, // Quantity of items
            });
        }

        options.Mode = "payment";
        options.SuccessUrl = "http://localhost:5016/Payment/success"; // Success URL after payment
        options.CancelUrl = "http://localhost:5016/Payment/cancel"; // Cancel URL if payment fails

        var service = new SessionService();
        Session session = service.Create(options);

        return Ok(new { id = session.Id });
    }
}

// Request model for creating checkout session
public class CreateCheckoutSessionRequest
{
    public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
}

// Model for items in the checkout session
public class CheckoutItem
{
    public string Title { get; set; } // Item title or product name
    public long Price { get; set; } // Price in the smallest currency unit (cents)
    public long Quantity { get; set; } // Quantity of the item
}
