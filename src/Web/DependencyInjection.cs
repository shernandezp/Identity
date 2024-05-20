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
                    //TODO: Separate certificates for signing and encryption
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
