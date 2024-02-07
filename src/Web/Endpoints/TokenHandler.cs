using Microsoft.AspNetCore;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace Security.Web.Endpoints;

public sealed class TokenHandler()
{
    public async Task<IResult> Exchange(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            throw new InvalidOperationException("The specified grant type is not supported.");

        // Retrieve the claims principal stored in the authorization code
        var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        return claimsPrincipal != null
            ? Results.SignIn(claimsPrincipal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
            : throw new NotImplementedException("The specified grant type is not implemented.");
    }
}
