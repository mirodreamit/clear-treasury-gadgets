using Microsoft.Azure.Functions.Worker.Http;

namespace CT.Gadgets.FunctionApp.Extensions;

public static class HttpResponseDataExtensions
{
    public static void SetRefreshTokenCookie(this HttpResponseData httpResponse, string refreshToken)
    {
        httpResponse.Headers.Add("Set-Cookie",
            $"refreshToken={refreshToken}; HttpOnly; Secure; SameSite=Strict; Path=/; Max-Age=604800");
    }
}
