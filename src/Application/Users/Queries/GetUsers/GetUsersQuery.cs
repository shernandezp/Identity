using System.Security.Authentication;
using Security.Domain.Interfaces;
using Security.Domain.Models;
using Common.Domain.Extensions;

namespace Security.Application.Users.Queries.GetUsers;

public record GetUsersQuery(string Email, string Password) : IRequest<UserVm>
{
    public string Email { get; set; } = Email;
    public string Password { get; set; } = Password;
}

public class GetUsersQueryHandler(IUserReader reader) : IRequestHandler<GetUsersQuery, UserVm>
{
    public async Task<UserVm> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var user = await reader.GetUserAsync(new Domain.Records.UserLoginDto(request.Email, request.Password), cancellationToken);

        if (user.Verified == null)
            throw new AuthenticationException("User account hasn't been verified");

        if (!user.Active)
            throw new AuthenticationException("User account is inactive");

        return user.Password.VerifyHashedPassword(request.Password)
            ? user : throw new AuthenticationException("Email or password is incorrect");
    }
}
