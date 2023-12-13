using Microsoft.AspNetCore;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Security.Application.Users.Queries.GetUsers;

namespace Security.Web.Endpoints;

public sealed class TokenHandler(IOpenIddictScopeManager scopeManager, ISender sender)
{
    public async Task<IResult> Exchange(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsClientCredentialsGrantType())
        {
            var query = new GetUsersQuery
            {
                UserName = request.Username,
                Password = request.Password
            };

            var user = await sender.Send(query) ?? 
                throw new UnauthorizedAccessException("The user is unauthorized.");

            var clientId = request.ClientId;
            var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType);
            // Override the user claims present in the principal in case they
            // changed since the authorization code/refresh token was issued.
            identity.SetClaim(Claims.Subject, clientId);
            identity.AddClaim(Claims.KeyId, $"{user.UserId}");
            user.Profiles.ToList().ForEach(item => identity.AddClaim(Claims.Profile, item.Name));
            user.Roles.ToList().ForEach(item => identity.AddClaim(Claims.Role, item.Name));
            identity.SetScopes(request.GetScopes());
            var principal = new ClaimsPrincipal(identity);
            principal.Claims.ToList().ForEach(claim => claim.SetDestinations(GetDestinations(claim, principal)));

            //populate audience (aud) claims based on requested scopes
            //ensure that you declared Resources while scope creating!
            principal.SetResources(await scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            context.User = principal;

            return Results.SignIn(new ClaimsPrincipal(identity), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code
            var claimsPrincipal = (await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (claimsPrincipal != null)
            {
                return Results.SignIn(claimsPrincipal, properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        }
        if (request.IsRefreshTokenGrantType())
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

    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;
            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
