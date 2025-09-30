using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class ScheduleApiService(IApiClient apiClient) : IScheduleApiService
{
  public async Task<IReadOnlyList<EmployeeScheduleSummary>> GetSchedulesAsync(
    string chargingName,
    DateOnly startDate,
    DateOnly endDate,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(chargingName))
    {
      throw new ArgumentException("Charging name must be provided", nameof(chargingName));
    }

    string requestUri = BuildListRequestUri(chargingName, startDate, endDate, tenant, skip, take);

    ScheduleListResponseDto? response = await apiClient.GetAsync<ScheduleListResponseDto>(requestUri, cancellationToken);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<EmployeeScheduleSummary>();
    }

    return response.Employees
      .Where(dto => dto is not null)
      .Select(MapToEmployeeSummary)
      .ToList();
  }

  public async Task<ScheduleSummary?> GetScheduleByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
  {
    if (scheduleId == Guid.Empty)
    {
      throw new ArgumentException("Schedule ID must be provided", nameof(scheduleId));
    }

    ScheduleDto? dto = await apiClient.GetAsync<ScheduleDto>(ScheduleRequestRoutes.BuildGetRoute(scheduleId), cancellationToken);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<Guid?> CreateScheduleAsync(ScheduleSummary schedule, CancellationToken cancellationToken = default)
  {
    if (schedule is null)
    {
      throw new ArgumentNullException(nameof(schedule));
    }

    if (schedule.EmployeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(schedule));
    }

    if (schedule.ShiftId == Guid.Empty)
    {
      throw new ArgumentException("Shift ID must be provided", nameof(schedule));
    }

    CreateScheduleRequestDto request = new()
    {
      EmployeeId = schedule.EmployeeId,
      ShiftId = schedule.ShiftId,
      ScheduleDate = schedule.ScheduleDate,
      IsDefault = schedule.IsDefault
    };

    CreateScheduleResponseDto? response = await apiClient.PostAsync<CreateScheduleRequestDto, CreateScheduleResponseDto>(
      CreateScheduleRequestDto.Route,
      request,
      cancellationToken);

    return response?.ScheduleId;
  }

  public async Task<ScheduleSummary?> UpdateScheduleAsync(ScheduleSummary schedule, CancellationToken cancellationToken = default)
  {
    if (schedule is null)
    {
      throw new ArgumentNullException(nameof(schedule));
    }

    if (schedule.Id == Guid.Empty)
    {
      throw new ArgumentException("Schedule ID must be provided", nameof(schedule));
    }

    if (schedule.EmployeeId == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(schedule));
    }

    if (schedule.ShiftId == Guid.Empty)
    {
      throw new ArgumentException("Shift ID must be provided", nameof(schedule));
    }

    UpdateScheduleRequestDto request = new()
    {
      ScheduleId = schedule.Id,
      EmployeeId = schedule.EmployeeId,
      ShiftId = schedule.ShiftId,
      ScheduleDate = schedule.ScheduleDate,
      IsDefault = schedule.IsDefault
    };

    UpdateScheduleResponseDto? response = await apiClient.PutAsync<UpdateScheduleRequestDto, UpdateScheduleResponseDto>(
      UpdateScheduleRequestDto.BuildRoute(schedule.Id),
      request,
      cancellationToken);

    return response?.Schedule is null ? null : MapToSummary(response.Schedule);
  }

  public Task<bool> DeleteScheduleAsync(Guid scheduleId, CancellationToken cancellationToken = default)
  {
    if (scheduleId == Guid.Empty)
    {
      throw new ArgumentException("Schedule ID must be provided", nameof(scheduleId));
    }

    return apiClient.DeleteAsync(ScheduleRequestRoutes.BuildDeleteRoute(scheduleId), cancellationToken);
  }

  private static string BuildListRequestUri(
    string chargingName,
    DateOnly startDate,
    DateOnly endDate,
    TenantDto tenant,
    int? skip,
    int? take)
  {
    List<string> parameters =
    [
      $"chargingName={Uri.EscapeDataString(chargingName)}",
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
    return $"Schedules?{query}";
  }

  private static ScheduleSummary MapToSummary(ScheduleDto dto)
  {
    return new ScheduleSummary
    {
      Id = dto.Id,
      EmployeeId = dto.EmployeeId,
      Name = dto.Name,
      ShiftId = dto.ShiftId,
      ScheduleDate = dto.ScheduleDate,
      IsDefault = dto.IsDefault,
      Shift = MapShift(dto.Shift)
    };
  }

  private static EmployeeScheduleSummary MapToEmployeeSummary(EmployeeScheduleDto dto)
  {
    return new EmployeeScheduleSummary
    {
      EmployeeId = dto.EmployeeId,
      Barcode = dto.Barcode,
      Name = dto.Name,
      Section = dto.Section,
      Charging = dto.Charging,
      Shifts = dto.Shifts?
        .Where(shift => shift is not null)
        .Select(MapShiftInfo)
        .ToList() ?? new List<ScheduleShiftInfoSummary>()
    };
  }

  private static ScheduleShiftDetailsSummary? MapShift(ScheduleShiftDetailsDto? dto)
  {
    if (dto is null)
    {
      return null;
    }

    return new ScheduleShiftDetailsSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      ShiftStartTime = dto.ShiftStartTime,
      ShiftBreakTime = dto.ShiftBreakTime,
      ShiftBreakEndTime = dto.ShiftBreakEndTime,
      ShiftEndTime = dto.ShiftEndTime,
      BreakHours = dto.BreakHours
    };
  }

  private static ScheduleShiftInfoSummary MapShiftInfo(ScheduleShiftInfoDto dto)
  {
    return new ScheduleShiftInfoSummary
    {
      ScheduleId = dto.ScheduleId,
      Shift = MapShift(dto.Shift),
      ScheduleDate = dto.ScheduleDate,
      IsDefault = dto.IsDefault,
      IsUpdated = dto.IsUpdated
    };
  }
}
