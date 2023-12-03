using Security.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Domain.Constants;

namespace Security.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<SecurityDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class SecurityDbContextInitialiser(ILogger<SecurityDbContextInitialiser> logger, SecurityDbContext context)
{
    private readonly ILogger<SecurityDbContextInitialiser> _logger = logger;
    private readonly SecurityDbContext _context = context;

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        if (!_context.Roles.Any())
        {
            _context.Roles.Add(new Role
            {
                Name = Roles.Administrator,
                Description = Roles.Administrator,
            });

            await _context.SaveChangesAsync();
        }

        if (!_context.Profiles.Any())
        {
            _context.Profiles.Add(new Profile
            {
                Name = Policies.CanPurge,
                Description = Policies.CanPurge,
            });

            await _context.SaveChangesAsync();
        }
    }
}
