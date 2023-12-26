using Microsoft.Extensions.Configuration;
using Security.Domain.Interfaces;
using Security.Infrastructure;
using Security.Infrastructure.Interfaces;
using Security.Infrastructure.Readers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Security");

        Guard.Against.Null(connectionString, message: "Connection string 'Security' not found.");

        services.AddDbContext<SecurityDbContext>((sp, options) => 
            options.UseNpgsql(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<ISecurityDbContext>(provider => provider.GetRequiredService<SecurityDbContext>());
        services.AddScoped<IUserReader, UserReader>();

        return services;
    }

    public static IServiceCollection AddOpenIdDictDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Security");

        Guard.Against.Null(connectionString, message: "Connection string 'Security' not found.");

        services.AddDbContext<AuthorityDbContext>(
            options => options.UseNpgsql(connectionString));

        return services;
    }
}
