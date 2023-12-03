using Security.Application.Common.Interfaces;
using Security.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        services.AddScoped<SecurityDbContextInitialiser>();

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
