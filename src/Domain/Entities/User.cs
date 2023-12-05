namespace Security.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public string? SecondName { get; set; }
    public required string LastName { get; set; }
    public string? SeconSurname { get; set; }
    public DateTime? DOB { get; set; }
    public IEnumerable<Role>? Roles { get; set; }
    public IEnumerable<Profile>? Profiles { get; set; }
}
