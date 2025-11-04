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

  Task<IReadOnlyList<EmployeeTimesheetSummary>> GetTimesheetSummariesAsync(
    string charging,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<bool> CreateTimesheetEntryAsync(
    string username,
    DateTime? timeInput,
    DateOnly entryDate,
    TimesheetInputType inputType,
    CancellationToken cancellationToken = default);

  Task<AttendanceTimesheetSummary?> UpdateTimesheetAsync(
    UpdateTimesheetRequestDto request,
    CancellationToken cancellationToken = default);
}
