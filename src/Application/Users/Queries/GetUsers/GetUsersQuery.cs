using AutoMapper;
using AutoMapper.QueryableExtensions;
using Security.Application.Common.Interfaces;

namespace Security.Application.Users.Queries.GetUsers;

public record GetUsersQuery() : IRequest<UserDto?>
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
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
