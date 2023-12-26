namespace Security.Domain.Models;
public record struct UserVm(
    Guid UserId,
    string Username,
    string Password,
    string Email,
    DateTime? Verified,
    bool Active);
