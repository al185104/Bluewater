using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class AttendanceApiService(IApiClient apiClient) : IAttendanceApiService
{
  public async Task<IReadOnlyList<AttendanceSummary>> GetAttendancesAsync(
    Guid employeeId,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    if (employeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(employeeId));
    }

    string requestUri = BuildListRequestUri(employeeId, startDate, endDate, skip, take);

    AttendanceListResponseDto? response = await apiClient.GetAsync<AttendanceListResponseDto>(requestUri, cancellationToken);

    if (response?.Attendances is not { Count: > 0 })
    {
      return Array.Empty<AttendanceSummary>();
    }

    return response.Attendances
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<IReadOnlyList<EmployeeAttendanceSummary>> GetAttendanceSummariesAsync(
    string charging,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(charging))
    {
      throw new ArgumentException("Charging must be provided", nameof(charging));
    }

    string requestUri = BuildListAllRequestUri(charging, startDate, endDate, tenant, skip, take);

    AttendanceListAllResponseDto? response = await apiClient.GetAsync<AttendanceListAllResponseDto>(requestUri, cancellationToken);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<EmployeeAttendanceSummary>();
    }

    return response.Employees
      .Where(dto => dto is not null)
      .Select(MapToEmployeeSummary)
      .ToList();
  }

  public async Task<AttendanceSummary?> GetAttendanceByIdAsync(Guid attendanceId, CancellationToken cancellationToken = default)
  {
    if (attendanceId == Guid.Empty)
    {
      throw new ArgumentException("Attendance ID must be provided", nameof(attendanceId));
    }

    AttendanceDto? dto = await apiClient.GetAsync<AttendanceDto>(
      AttendanceRequestRoutes.BuildGetRoute(attendanceId),
      cancellationToken);

    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<AttendanceSummary?> CreateAttendanceAsync(AttendanceSummary attendance, CancellationToken cancellationToken = default)
  {
    if (attendance is null)
    {
      throw new ArgumentNullException(nameof(attendance));
    }

    if (attendance.EmployeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(attendance));
    }

    AttendanceCreateRequestDto request = new()
    {
      EmployeeId = attendance.EmployeeId,
      ShiftId = attendance.ShiftId,
      TimesheetId = attendance.TimesheetId,
      LeaveId = attendance.LeaveId,
      EntryDate = attendance.EntryDate,
      WorkHrs = attendance.WorkHours,
      LateHrs = attendance.LateHours,
      UnderHrs = attendance.UnderHours,
      OverbreakHrs = attendance.OverbreakHours,
      NightShiftHrs = attendance.NightShiftHours,
      IsLocked = attendance.IsLocked
    };

    AttendanceCreateResponseDto? response = await apiClient.PostAsync<AttendanceCreateRequestDto, AttendanceCreateResponseDto>(
      AttendanceCreateRequestDto.Route,
      request,
      cancellationToken);

    return response?.Attendance is null ? null : MapToSummary(response.Attendance);
  }

  public async Task<AttendanceSummary?> UpdateAttendanceAsync(AttendanceSummary attendance, CancellationToken cancellationToken = default)
  {
    if (attendance is null)
    {
      throw new ArgumentNullException(nameof(attendance));
    }

    if (attendance.EmployeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(attendance));
    }

    if (attendance.EntryDate is null)
    {
      throw new ArgumentException("Entry date must be provided", nameof(attendance));
    }

    AttendanceUpdateRequestDto request = new()
    {
      EmployeeId = attendance.EmployeeId,
      EntryDate = attendance.EntryDate,
      ShiftId = attendance.ShiftId,
      TimesheetId = attendance.TimesheetId,
      LeaveId = attendance.LeaveId,
      IsLocked = attendance.IsLocked
    };

    AttendanceUpdateResponseDto? response = await apiClient.PutAsync<AttendanceUpdateRequestDto, AttendanceUpdateResponseDto>(
      AttendanceUpdateRequestDto.Route,
      request,
      cancellationToken);

    return response?.Attendance is null ? null : MapToSummary(response.Attendance);
  }

  public Task<bool> DeleteAttendanceAsync(Guid attendanceId, CancellationToken cancellationToken = default)
  {
    if (attendanceId == Guid.Empty)
    {
      throw new ArgumentException("Attendance ID must be provided", nameof(attendanceId));
    }

    return apiClient.DeleteAsync(AttendanceRequestRoutes.BuildDeleteRoute(attendanceId), cancellationToken);
  }

  private static string BuildListRequestUri(Guid employeeId, DateOnly startDate, DateOnly endDate, int? skip, int? take)
  {
    List<string> parameters =
    [
      $"employeeId={employeeId}",
      $"startDate={startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"endDate={endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}"
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
    return $"Attendances?{query}";
  }

  private static string BuildListAllRequestUri(
    string charging,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip,
    int? take)
  {
    List<string> parameters =
    [
      $"charging={Uri.EscapeDataString(charging)}",
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
    return $"Attendances/All?{query}";
  }

  private static AttendanceSummary MapToSummary(AttendanceDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new AttendanceSummary
    {
      Id = dto.Id,
      EmployeeId = dto.EmployeeId,
      ShiftId = dto.ShiftId,
      TimesheetId = dto.TimesheetId,
      LeaveId = dto.LeaveId,
      EntryDate = dto.EntryDate,
      WorkHours = dto.WorkHrs,
      LateHours = dto.LateHrs,
      UnderHours = dto.UnderHrs,
      OverbreakHours = dto.OverbreakHrs,
      NightShiftHours = dto.NightShiftHours,
      IsLocked = dto.IsLocked,
      Shift = MapShift(dto.Shift),
      Timesheet = MapTimesheet(dto.Timesheet)
    };
  }

  private static AttendanceShiftSummary? MapShift(AttendanceShiftDto? shift)
  {
    if (shift is null)
    {
      return null;
    }

    return new AttendanceShiftSummary
    {
      Id = shift.Id,
      Name = shift.Name,
      ShiftStartTime = shift.ShiftStartTime,
      ShiftBreakTime = shift.ShiftBreakTime,
      ShiftBreakEndTime = shift.ShiftBreakEndTime,
      ShiftEndTime = shift.ShiftEndTime,
      BreakHours = shift.BreakHours
    };
  }

  private static AttendanceTimesheetSummary? MapTimesheet(AttendanceTimesheetDto? timesheet)
  {
    if (timesheet is null)
    {
      return null;
    }

    return new AttendanceTimesheetSummary
    {
      Id = timesheet.Id,
      EmployeeId = timesheet.EmployeeId,
      TimeIn1 = timesheet.TimeIn1,
      TimeOut1 = timesheet.TimeOut1,
      TimeIn2 = timesheet.TimeIn2,
      TimeOut2 = timesheet.TimeOut2,
      EntryDate = timesheet.EntryDate,
      IsEdited = timesheet.IsEdited
    };
  }

  private static EmployeeAttendanceSummary MapToEmployeeSummary(EmployeeAttendanceDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new EmployeeAttendanceSummary
    {
      EmployeeId = dto.EmployeeId,
      Barcode = dto.Barcode,
      Name = dto.Name,
      Department = dto.Department,
      Section = dto.Section,
      Charging = dto.Charging,
      Attendances = dto.Attendances?
        .Where(attendance => attendance is not null)
        .Select(MapToSummary)
        .ToList() ?? new List<AttendanceSummary>(),
      TotalWorkHours = dto.TotalWorkHrs,
      TotalAbsences = dto.TotalAbsences,
      TotalLateHours = dto.TotalLateHrs,
      TotalUnderHours = dto.TotalUnderHrs,
      TotalOverbreakHours = dto.TotalOverbreakHrs,
      TotalNightShiftHours = dto.TotalNightShiftHrs,
      TotalLeaves = dto.TotalLeaves
    };
  }
}
