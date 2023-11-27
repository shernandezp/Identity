namespace OpenIdDictSample.Server.Controllers
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Mvc;
    using OpenIddict.Server.AspNetCore;
    using static OpenIddict.Abstractions.OpenIddictConstants;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using OpenIddict.Abstractions;
    using Microsoft.AspNetCore.Authentication;


    public class AuthorizationController(IOpenIddictScopeManager scopeManager) : Controller
    {
        [HttpPost("~/token")]
        public async ValueTask<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsClientCredentialsGrantType())
            {
                /*Validate user and password here*/


                var clientId = request.ClientId;
                var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType);
                // Override the user claims present in the principal in case they
                // changed since the authorization code/refresh token was issued.
                identity.SetClaim(Claims.Subject, clientId);
                identity.AddClaim(Claims.Username, request.Username);
                identity.AddClaim(Claims.Role, "Administrator");
                identity.SetScopes(request.GetScopes());
                var principal = new ClaimsPrincipal(identity);

                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                //populate audience (aud) claims based on requested scopes
                //ensure that you declared Resources while scope creating!
                principal.SetResources(await scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            if (request.IsRefreshTokenGrantType())
            {
                var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
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
}
