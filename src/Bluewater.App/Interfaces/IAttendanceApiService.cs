using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IAttendanceApiService
{
  Task<IReadOnlyList<AttendanceSummary>> GetAttendancesAsync(
    Guid employeeId,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<PagedResult<EmployeeAttendanceSummary>> GetAttendanceSummariesAsync(
    string charging,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);

  Task<AttendanceSummary?> GetAttendanceByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default);

  Task<AttendanceSummary?> CreateAttendanceAsync(AttendanceSummary attendance, CancellationToken cancellationToken = default);

  Task<AttendanceSummary?> UpdateAttendanceAsync(AttendanceSummary attendance, CancellationToken cancellationToken = default);

  Task<bool> DeleteAttendanceAsync(Guid attendanceId, CancellationToken cancellationToken = default);
}
