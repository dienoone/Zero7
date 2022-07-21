using Twilio.Rest.Api.V2010.Account;

namespace O7.Core.Interfaces
{
    public interface ISMSService
    {
        MessageResource Send(string mobileNumber, string body);
    }
}
