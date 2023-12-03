using OpenIddict.Abstractions;
using Security.Infrastructure.Data;

namespace Security.Web;

public sealed class ClientSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var databaseContext = scope.ServiceProvider.GetRequiredService<AuthorityDbContext>();
        databaseContext.Database.EnsureCreated();

        await PopulateScopes(scope, cancellationToken);

        await PopulateInternalApps(scope, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async ValueTask PopulateScopes(IServiceScope scope, CancellationToken cancellationToken)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var scopeDescriptor = new OpenIddictScopeDescriptor
        {
            Name = "mobile_scope",
            Resources = { "mobile_resource" }
        };

        var scopeInstance = await scopeManager.FindByNameAsync(scopeDescriptor.Name, cancellationToken);

        if (scopeInstance == null)
        {
            await scopeManager.CreateAsync(scopeDescriptor, cancellationToken);
        }
        else
        {
            await scopeManager.UpdateAsync(scopeInstance, scopeDescriptor, cancellationToken);
        }
    }

    private static async ValueTask PopulateInternalApps(IServiceScope scopeService, CancellationToken cancellationToken)
    {
        var appManager = scopeService.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var appDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "mobile_client",
            ClientSecret = "1b01a2f0-01ef-482b-ade2-34a251632ef7",
            Type = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.Introspection,
                        OpenIddictConstants.Permissions.Endpoints.Revocation,

                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "mobile_scope"
                    }
        };

        var client = await appManager.FindByClientIdAsync(appDescriptor.ClientId, cancellationToken);

        if (client == null)
        {
            await appManager.CreateAsync(appDescriptor, cancellationToken);
        }
        else
        {
            await appManager.UpdateAsync(client, appDescriptor, cancellationToken);
        }
    }
}
