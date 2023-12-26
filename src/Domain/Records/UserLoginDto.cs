namespace Security.Domain.Records;
public record struct UserLoginDto(
    string Email,
    string Password);
