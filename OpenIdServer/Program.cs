using Microsoft.EntityFrameworkCore;
using OpenIdDictSample.Server;
using OpenIdServer.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("OAuthServer"));
        options.UseOpenIddict<long>();
    });

builder.Services.AddOpenIddict()
    .AddCore(
    _ =>
    {
        _.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>()
            .ReplaceDefaultEntities<long>();
    })
    .AddServer(
    _ =>
    {
        _.AllowClientCredentialsFlow();
        _.AllowRefreshTokenFlow();

        _.SetTokenEndpointUris("token");
        _.SetRevocationEndpointUris("token/revoke");
        _.SetIntrospectionEndpointUris("token/introspect");
        _.RegisterScopes("mobile_scope");

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

builder.Services.AddHostedService<ClientSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();