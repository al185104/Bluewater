using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface ILeaveApiService
{
  Task<IReadOnlyList<LeaveSummary>> GetLeavesAsync(
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);

  Task<LeaveSummary?> GetLeaveByIdAsync(Guid leaveId, CancellationToken cancellationToken = default);

  Task<LeaveSummary?> CreateLeaveAsync(LeaveSummary leave, CancellationToken cancellationToken = default);

  Task<LeaveSummary?> UpdateLeaveAsync(LeaveSummary leave, CancellationToken cancellationToken = default);

  Task<bool> DeleteLeaveAsync(Guid leaveId, CancellationToken cancellationToken = default);
}
