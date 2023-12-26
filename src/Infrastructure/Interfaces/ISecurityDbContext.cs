using Security.Infrastructure.Entities;

namespace Security.Infrastructure.Interfaces;
public interface ISecurityDbContext
{
    DbSet<User> Users { get; set; }
}
