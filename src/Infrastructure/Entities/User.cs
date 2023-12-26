using Common.Infrastructure;

namespace Security.Infrastructure.Entities;
public sealed class User(
    string username,
    string password,
    string email) : BaseAuditableEntity
{
    public Guid UserId { get; private set; } = Guid.NewGuid();
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Email { get; set; } = email;
    public DateTime? Verified { get; set; }
    public bool Active { get; set; }
}
