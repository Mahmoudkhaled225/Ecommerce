using C_ServiceLayer.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;

namespace C_ServiceLayer.Concretes;

public class PaymentService : IPaymentService
{
    private readonly Payment _options;
    private readonly IConfiguration _configuration;
    

    public PaymentService(IOptions<Payment> options, IConfiguration configuration)
    {
        _options = options.Value;
        _configuration = configuration;
    }
    
    public async Task<string?> CreatePaymentIntent(long amount)
    {
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        var service = new PaymentIntentService();
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" },
        };
        var paymentIntent = await service.CreateAsync(options);
        return paymentIntent.ClientSecret;
    } 
    
}