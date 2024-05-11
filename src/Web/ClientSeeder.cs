// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using OpenIddict.Abstractions;
using Security.Infrastructure;

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
        await PopulateScope(scope, "mobile_scope", "mobile_resource", cancellationToken);
        await PopulateScope(scope, "web_scope", "web_resource", cancellationToken);
    }

    private static async ValueTask PopulateScope(IServiceScope scope, 
        string scopeName,
        string resource,
        CancellationToken cancellationToken)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var scopeDescriptor = new OpenIddictScopeDescriptor
        {
            Name = scopeName,
            Resources = { resource }
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
        await PopulateInternalApp(scopeService, "mobile_client", "1b01a2f0-01ef-482b-ade2-34a251632ef7", "https://oauth.pstmn.io/v1/callback", "mobile_scope", cancellationToken);
        await PopulateInternalApp(scopeService, "web_client", "470339fa-040e-4a43-b410-e3e4bc55c858", "http://localhost:3000/authentication/callback", "web_scope", cancellationToken);
    }

    private static async ValueTask PopulateInternalApp(IServiceScope scopeService,
        string clientId,
        string clientSecret,
        string uri,
        string scope,
        CancellationToken cancellationToken)
    {
        var appManager = scopeService.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var appDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            //ClientSecret = clientSecret,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            RedirectUris = { new Uri(uri) },
            Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.Introspection,
                        OpenIddictConstants.Permissions.Endpoints.Revocation,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.Prefixes.Scope + scope,
                        OpenIddictConstants.Permissions.ResponseTypes.Code
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
