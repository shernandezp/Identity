using Security.Domain.Entities;

namespace Security.Application.Users.Queries.GetUsers;
public sealed class UserDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public IReadOnlyCollection<RoleDto> Roles { get; init; } = Array.Empty<RoleDto>();
    public IReadOnlyCollection<ProfileDto> Profiles { get; init; } = Array.Empty<ProfileDto>();

    private class Mapping : AutoMapper.Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}
