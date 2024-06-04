namespace C_ServiceLayer.Abstractions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


public interface ISmsService
{
    Task<MessageResource?> Send(string phone, string? body);
}