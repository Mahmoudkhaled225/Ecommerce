namespace C_ServiceLayer.Abstractions;

public interface IPaymentService
{
    Task<string?> CreatePaymentIntent( long amount);
}