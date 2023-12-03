namespace Security.Domain.Entities;
public class Role
{
    public int RoleId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<User>? Users { get; set; }
}
