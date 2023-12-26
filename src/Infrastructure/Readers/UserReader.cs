using Security.Domain.Interfaces;
using Security.Domain.Models;
using Security.Domain.Records;

namespace Security.Infrastructure.Readers;
public sealed class UserReader(SecurityDbContext context) : IUserReader
{
    public async Task<UserVm> GetUserAsync(UserLoginDto userLogin, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Email.Equals(userLogin.Email))
            .Select(u => new UserVm(
                u.UserId,
                u.Username,
                u.Password,
                u.Email,
                u.Verified,
                u.Active))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
