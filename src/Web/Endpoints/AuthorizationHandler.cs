using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore;

namespace Security.Web.Endpoints;

public sealed class AuthorizationHandler
{
    public async Task Authorize(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // If the user principal can't be extracted, throw an exception.
        if (!result.Succeeded)
        {
            //throw new UnauthorizedAccessException("The user is unauthorized.");
            await context.ChallengeAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties
                    {
                        RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(
                        context.Request.HasFormContentType ? [.. context.Request.Form] : context.Request.Query.ToList())
                    });
            return;
        }

        var claims = new List<Claim>
        {
            new(OpenIddictConstants.Claims.Subject, result.Principal?.Identity?.Name ?? string.Empty),
            //new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken)
        };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        claimsPrincipal.SetScopes(request.GetScopes());

        if (claimsPrincipal != null)
        {
            //context.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
            await context.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }
}
