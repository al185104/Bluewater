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

    string requestUri = BuildListAllRequestUri(startDate, endDate, tenant, skip, take, charging: null);

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

  public async Task<IReadOnlyList<EmployeeTimesheetSummary>> GetTimesheetSummariesAsync(
    string charging,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(charging))
    {
      throw new ArgumentException("Charging must be provided", nameof(charging));
    }

    string requestUri = BuildListAllRequestUri(startDate, endDate, tenant, skip, take, charging);

    TimesheetListAllResponseDto? response = await apiClient
      .GetAsync<TimesheetListAllResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<EmployeeTimesheetSummary>();
    }

    return response.Employees
      .Where(employee => employee is not null)
      .Select(employee => MapToEmployeeSummary(employee!))
      .Where(summary => summary.Timesheets.Count > 0)
      .OrderBy(summary => summary.Name, StringComparer.OrdinalIgnoreCase)
      .ToList();
  }

  public async Task<bool> CreateTimesheetEntryAsync(
    string username,
    DateTime? timeInput,
    DateOnly entryDate,
    TimesheetInputType inputType,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(username))
    {
      throw new ArgumentException("Username must be provided", nameof(username));
    }

    CreateTimesheetRequestDto request = new()
    {
      Username = username,
      TimeInput = timeInput,
      EntryDate = entryDate,
      InputType = (int)inputType
    };

    CreateTimesheetResponseDto? response = await apiClient
      .PostAsync<CreateTimesheetRequestDto, CreateTimesheetResponseDto>(CreateTimesheetRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response is not null;
  }

  private static string BuildListAllRequestUri(
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip,
    int? take,
    string? charging)
  {
    List<string> parameters =
    [
      $"startDate={startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"endDate={endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"tenant={tenant}"
    ];

    if (!string.IsNullOrWhiteSpace(charging))
    {
      parameters.Add($"charging={Uri.EscapeDataString(charging)}");
    }

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

  public async Task<AttendanceTimesheetSummary?> UpdateTimesheetAsync(
    UpdateTimesheetRequestDto request,
    CancellationToken cancellationToken = default)
  {
    if (request is null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    UpdateTimesheetResponseDto? response = await apiClient
      .PutAsync<UpdateTimesheetRequestDto, UpdateTimesheetResponseDto>("Timesheets", request, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Timesheet is null)
    {
      return null;
    }

    TimesheetDto dto = response.Timesheet;

    return new AttendanceTimesheetSummary
    {
      Id = dto.Id,
      EmployeeId = dto.EmployeeId,
      TimeIn1 = dto.TimeIn1,
      TimeOut1 = dto.TimeOut1,
      TimeIn2 = dto.TimeIn2,
      TimeOut2 = dto.TimeOut2,
      EntryDate = dto.EntryDate,
      IsEdited = dto.IsEdited
    };
  }

  private static EmployeeTimesheetSummary MapToEmployeeSummary(AllEmployeeTimesheetDto dto)
  {
    var summary = new EmployeeTimesheetSummary
    {
      EmployeeId = dto.EmployeeId,
      Name = dto.Name,
      Department = dto.Department,
      Section = dto.Section,
      Charging = dto.Charging,
      TotalWorkHours = dto.TotalWorkHours,
      TotalBreak = dto.TotalBreak,
      TotalLates = dto.TotalLates,
      TotalAbsents = dto.TotalAbsents
    };

    if (dto.Timesheets is { Count: > 0 })
    {
      IEnumerable<AttendanceTimesheetSummary> timesheets = dto.Timesheets
        .Where(timesheet => timesheet is not null)
        .Select(timesheet => MapToSummary(dto.EmployeeId, timesheet!))
        .OrderByDescending(timesheet => timesheet.EntryDate);

      foreach (AttendanceTimesheetSummary timesheet in timesheets)
      {
        summary.Timesheets.Add(timesheet);
      }
    }

    return summary;
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
