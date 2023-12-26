using Security.Infrastructure.Configurations;
using Security.Infrastructure.Entities;
using Security.Infrastructure.Interfaces;

namespace Security.Infrastructure;

public class SecurityDbContext : DbContext, ISecurityDbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfiguration());
    }
}
