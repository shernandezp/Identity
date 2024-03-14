// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
