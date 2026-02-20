using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IUserApiService
{
  Task<IReadOnlyList<UserRecordDto>> GetUsersAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<UserRecordDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<UserRecordDto?> CreateUserAsync(UserRecordDto user, CancellationToken cancellationToken = default);

  Task<UserRecordDto?> UpdateUserAsync(UserRecordDto user, CancellationToken cancellationToken = default);

  Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
