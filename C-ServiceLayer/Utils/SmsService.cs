using C_ServiceLayer.Abstractions;
using C_ServiceLayer.Concretes;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace C_ServiceLayer.Utils;

public class SmsService : ISmsService
{
    private readonly Sms _options;

    public SmsService(IOptions<Sms> options) =>
        _options = options.Value;


    public async Task<MessageResource?> Send(string phone, string? body)
    {
        TwilioClient.Init(_options.AccountSID, _options.AuthToken);
        var result = await MessageResource.CreateAsync(
            body: body,
            from: new Twilio.Types.PhoneNumber(_options.TwilioPhoneNumber),
            to: phone);

        return result;
    }
}
