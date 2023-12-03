namespace Security.Domain.Entities;
public sealed class Profile
{
    public int ProfileId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<User>? Users { get; set; }
}
