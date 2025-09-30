using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IScheduleApiService
{
  Task<IReadOnlyList<EmployeeScheduleSummary>> GetSchedulesAsync(
    string chargingName,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);

  Task<ScheduleSummary?> GetScheduleByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);

  Task<Guid?> CreateScheduleAsync(ScheduleSummary schedule, CancellationToken cancellationToken = default);

  Task<ScheduleSummary?> UpdateScheduleAsync(ScheduleSummary schedule, CancellationToken cancellationToken = default);

  Task<bool> DeleteScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default);
}
