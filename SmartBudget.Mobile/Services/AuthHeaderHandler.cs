using System.Net.Http.Headers;
using SmartBudget.Mobile.Helpers;

namespace SmartBudget.Mobile.Services;

// Automatically attaches the JWT to every outgoing request.
public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await SecureStorage.GetAsync(Constants.AuthTokenKey);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch { /* no token yet — request goes out unauthenticated */ }

        return await base.SendAsync(request, cancellationToken);
    }
}