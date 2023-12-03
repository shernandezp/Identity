using Security.Domain.Entities;

namespace Security.Application.Users.Queries.GetUsers;
public sealed class RoleDto
{
    public required string Name { get; init; }

    private class Mapping : AutoMapper.Profile
    {
        public Mapping()
        {
            CreateMap<Role, RoleDto>();
        }
    }
}
