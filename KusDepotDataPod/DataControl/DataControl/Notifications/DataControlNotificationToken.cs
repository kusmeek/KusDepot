namespace DataPodServices.DataControl;

internal static class DataControlNotificationToken
{
    internal const String AuthorizationTokenClaimType = "urn:kusdepot:datacontrol:authorization-token";

    internal static String Get(HttpContext? context)
    {
        if(context is null) { return String.Empty; }

        String authorization = context.Request.Headers.Authorization.ToString();

        if(authorization.StartsWith("Bearer ",OrdinalIgnoreCase))
        {
            String token = authorization[7..].Trim();

            if(String.IsNullOrWhiteSpace(token) is false) { return token; }
        }

        return context.Request.Query["access_token"].ToString().Trim();
    }

    internal static String Get(ClaimsPrincipal? principal , HttpContext? context)
    {
        String? token = principal?.FindFirst(AuthorizationTokenClaimType)?.Value;

        return String.IsNullOrWhiteSpace(token) ? Get(context) : token;
    }
}
