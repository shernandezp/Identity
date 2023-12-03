using Security.Application.Common.Interfaces;
using Security.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Security.Infrastructure.Data.Configurations.Security;

namespace Security.Infrastructure.Data;

public class SecurityDbContext : DbContext, ISecurityDbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ProfileConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new UserProfileConfiguration());
        builder.ApplyConfiguration(new UserRoleConfiguration());
    }
}
