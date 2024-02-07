using System.Security.Cryptography.X509Certificates;
using Quartz;
using Security.Infrastructure;

namespace Security.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIdDictServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
#pragma warning disable CS0618 // Type or member is obsolete
            options.UseMicrosoftDependencyInjectionJobFactory();
#pragma warning restore CS0618 // Type or member is obsolete
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddOpenIddict()
        .AddCore(options => {
            options.UseEntityFrameworkCore()
            .UseDbContext<AuthorityDbContext>()
            .ReplaceDefaultEntities<long>();
            options.UseQuartz();
        })
        .AddServer(
            _ =>
            {
                _.AllowClientCredentialsFlow();
                _.AllowRefreshTokenFlow();
                _.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

                _.SetAuthorizationEndpointUris("authorize");
                _.SetTokenEndpointUris("token");
                _.SetRevocationEndpointUris("token/revoke");
                _.SetIntrospectionEndpointUris("token/introspect");
                _.SetLogoutEndpointUris("logout");
                _.RegisterScopes("mobile_scope", "web_scope", "sec_scope");

#if DEBUG
                    _.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
#else
                    var certificatePath = configuration["OpenIddict:Path"];
                    var certificatePassword = configuration["OpenIddict:Password"];

                    var bytes = File.ReadAllBytes(certificatePath ?? "");
                    var certificate = new X509Certificate2(
                        bytes,
                        certificatePassword);
                    _.AddSigningCertificate(certificate)
                        .AddEncryptionCertificate(certificate);
#endif

                ////disable access token payload encryption
                _.DisableAccessTokenEncryption();
                _.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();
            }
        );


        return services;
    }
}
