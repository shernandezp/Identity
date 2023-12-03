using Security.Infrastructure.Data;

namespace Security.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIdDictServices(this IServiceCollection services)
    {
        services.AddOpenIddict()
        .AddCore(
            _ => _.UseEntityFrameworkCore()
                    .UseDbContext<AuthorityDbContext>()
                    .ReplaceDefaultEntities<long>())
        .AddServer(
            _ =>
            {
                _.AllowClientCredentialsFlow();
                _.AllowRefreshTokenFlow();

                _.SetTokenEndpointUris("token");
                _.SetRevocationEndpointUris("token/revoke");
                _.SetIntrospectionEndpointUris("token/introspect");
                _.RegisterScopes("mobile_scope", "web_scope", "sec_scope");

                //don't use this in porduction
                _.AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();

                ////Production
                ////register real certificate here
                ////like
                ////should be cert in PKCE format
                //var cerdata = builder.Configuration["OpenIddict:Certificate"];
                //var cer = new X509Certificate2(
                //    Convert.FromBase64String(cerdata),
                //    password: (string)null,
                ////it is important to use X509KeyStorageFlags.EphemeralKeySet to avoid 
                ////Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException: The system cannot find the file specified.
                //    keyStorageFlags: X509KeyStorageFlags.EphemeralKeySet
                //    );
                //_.AddSigningCertificate(cer)
                //    .AddEncryptionCertificate(cer);

                ////disable access token payload encryption
                _.DisableAccessTokenEncryption();
                _.UseAspNetCore().EnableTokenEndpointPassthrough();
            }
        );

        return services;
    }
}
