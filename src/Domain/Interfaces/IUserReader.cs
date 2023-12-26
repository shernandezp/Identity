using Security.Domain.Models;
using Security.Domain.Records;

namespace Security.Domain.Interfaces;
public interface IUserReader
{
    Task<UserVm> GetUserAsync(UserLoginDto user, CancellationToken cancellationToken = default);
}
