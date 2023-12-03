namespace Security.Domain.Entities;
public class UserRole
{
    public required Guid UserId { get; set; }
    public required int RoleId { get; set; }
}
