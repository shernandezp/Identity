using Microsoft.EntityFrameworkCore;

namespace Security.Infrastructure;
public class AuthorityDbContext : DbContext
{
    public AuthorityDbContext(DbContextOptions<AuthorityDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseOpenIddict<long>();
    }
}
