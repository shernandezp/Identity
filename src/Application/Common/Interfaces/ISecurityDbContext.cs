using Security.Domain.Entities;

namespace Security.Application.Common.Interfaces;
public interface ISecurityDbContext
{
    DbSet<Profile> Profiles { get; set; }
    DbSet<Role> Roles { get; set; }
    DbSet<UserProfile> UserProfiles { get; set; }
    DbSet<UserRole> UserRoles { get; set; }
    DbSet<User> Users { get; set; }
}
