namespace Security.Domain.Entities;
public class UserProfile
{
    public required Guid UserId { get; set; }
    public required int ProfileId { get; set; }
}
