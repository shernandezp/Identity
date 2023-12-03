using AutoMapper;
using AutoMapper.QueryableExtensions;
using Security.Application.Common.Interfaces;

namespace Security.Application.Users.Queries.GetUsers;

public record GetUsersQuery (string username, string password) : IRequest<UserDto?>
{
    public string UserName { get; init; } = username;
    public string Password { get; init; } = password;
}

public class GetUsersQueryHandler(ISecurityDbContext context, IMapper mapper) : IRequestHandler<GetUsersQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await context.Users
            .Where(u => u.Username.Equals(request.UserName) && u.Password.Equals(request.Password))
            .Include(role => role.Roles)
            .Include(user => user.Profiles)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
