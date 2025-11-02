using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class TimesheetApiService(IApiClient apiClient) : ITimesheetApiService
{
  public async Task<IReadOnlyList<AttendanceTimesheetSummary>> GetTimesheetsAsync(
    Guid employeeId,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    if (employeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(employeeId));
    }

    string requestUri = BuildListAllRequestUri(startDate, endDate, tenant, skip, take);

    TimesheetListAllResponseDto? response = await apiClient
      .GetAsync<TimesheetListAllResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<AttendanceTimesheetSummary>();
    }

    AllEmployeeTimesheetDto? employee = response.Employees
      .Where(item => item is not null && item.EmployeeId == employeeId)
      .Select(item => item!)
      .FirstOrDefault();

    if (employee?.Timesheets is not { Count: > 0 })
    {
      return Array.Empty<AttendanceTimesheetSummary>();
    }

    return employee.Timesheets
      .Where(dto => dto is not null)
      .Select(dto => MapToSummary(employee.EmployeeId, dto!))
      .OrderByDescending(timesheet => timesheet.EntryDate)
      .ToList();
  }

  private static string BuildListAllRequestUri(
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip,
    int? take)
  {
    List<string> parameters =
    [
      $"startDate={startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"endDate={endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"tenant={tenant}"
    ];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    string query = string.Join('&', parameters);
    return $"Timesheets/All?{query}";
  }

  private static AttendanceTimesheetSummary MapToSummary(Guid employeeId, TimesheetInfoDto dto)
  {
    return new AttendanceTimesheetSummary
    {
      Id = dto.TimesheetId,
      EmployeeId = employeeId,
      TimeIn1 = dto.TimeIn1,
      TimeOut1 = dto.TimeOut1,
      TimeIn2 = dto.TimeIn2,
      TimeOut2 = dto.TimeOut2,
      EntryDate = dto.EntryDate,
      IsEdited = dto.IsEdited
    };
  }
}
