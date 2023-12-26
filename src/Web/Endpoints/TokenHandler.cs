using Microsoft.AspNetCore;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace Security.Web.Endpoints;

public sealed class TokenHandler(IOpenIddictScopeManager scopeManager)
{
    public async Task<IResult> Exchange(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsClientCredentialsGrantType())
        {
            var clientId = request.ClientId;
            var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType);
            // Override the user claims
            identity.SetClaim(Claims.Subject, clientId);
            identity.SetScopes(request.GetScopes());
            var principal = new ClaimsPrincipal(identity);

            //populate audience
            principal.SetResources(await scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());
            return Results.SignIn(principal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }
        else if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code
            var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (claimsPrincipal != null)
            {
                return Results.SignIn(claimsPrincipal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        }
        else if (request.IsRefreshTokenGrantType())
        {
            var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (claimsPrincipal != null)
            {
                context.User = claimsPrincipal;
                return Results.SignIn(new ClaimsPrincipal(claimsPrincipal), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }
}
