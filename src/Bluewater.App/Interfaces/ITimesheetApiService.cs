using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface ITimesheetApiService
{
  Task<IReadOnlyList<AttendanceTimesheetSummary>> GetTimesheetsAsync(
    Guid employeeId,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);
}
