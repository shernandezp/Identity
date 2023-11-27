namespace OpenIdServer
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using OpenIddict.Abstractions;
    using OpenIddict.Server;
    using static OpenIddict.Server.OpenIddictServerEvents;

    public class TokenRequestHandler : IOpenIddictServerHandler<HandleTokenRequestContext>
    {
        public Task HandleAsync(HandleTokenRequestContext context)
        {
            if (context.Request.IsAuthorizationCodeGrantType())
            {
                // Obtener el usuario para agregar los claims
                var user = context.Principal;

                if (user != null)
                {
                    // Agregar claims personalizados al token
                    ((ClaimsIdentity)user.Identity).AddClaim(new Claim("custom_claim", "value"));

                    // Asignar el principal actualizado al contexto de la respuesta del token
                    context.Principal = user;
                }
            }

            return Task.CompletedTask;
        }
        ValueTask IOpenIddictServerHandler<HandleTokenRequestContext>.HandleAsync(HandleTokenRequestContext context)
        {
            throw new NotImplementedException();
        }

    }
    /*public class AuthorizationServer : OpenIddictServerProvider
    {
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            // Validaciones personalizadas para la solicitud de token
            await base.ValidateTokenRequest(context);
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            // Validaciones y manejo de la solicitud de token (por ejemplo, la emisión del token)
            await base.HandleTokenRequest(context);
        }

        public override async Task HandleAuthorizationRequest(HandleAuthorizationRequestContext context)
        {
            // Manejo de la solicitud de autorización (por ejemplo, validar el cliente, el alcance, etc.)
            await base.HandleAuthorizationRequest(context);
        }

        public override Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            // Agregar roles e identificador de usuario como claims en el token de respuesta
            if (context.Request.AccessTokenRequested)
            {
                var principal = context.Ticket.Principal;

                // Agregar los roles como claims al token
                var roles = new[] { "admin", "user" }; // Obtén los roles del usuario desde tu base de datos o almacenamiento
                ((ClaimsIdentity)principal.Identity).AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // Agregar el identificador de usuario como claim al token
                var userId = "123456"; // Obtén el identificador del usuario
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("user_id", userId));
            }

            return base.ApplyTokenResponse(context);
        }
    }*/
}
