using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class ShiftApiService(IApiClient apiClient) : IShiftApiService
{
  public async Task<IReadOnlyList<ShiftSummary>> GetShiftsAsync(CancellationToken cancellationToken = default)
  {
    ShiftListResponseDto? response = await apiClient.GetAsync<ShiftListResponseDto>("Shifts", cancellationToken);

    if (response?.Shifts is not { Count: > 0 })
    {
      return Array.Empty<ShiftSummary>();
    }

    return response.Shifts
      .Where(shift => shift is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<ShiftSummary?> CreateShiftAsync(ShiftSummary shift, CancellationToken cancellationToken = default)
  {
    CreateShiftRequestDto request = new()
    {
      Name = shift.Name,
      ShiftStartTime = ParseTimeOrNull(shift.ShiftStartTime),
      ShiftBreakTime = ParseTimeOrNull(shift.ShiftBreakTime),
      ShiftBreakEndTime = ParseTimeOrNull(shift.ShiftBreakEndTime),
      ShiftEndTime = ParseTimeOrNull(shift.ShiftEndTime),
      BreakHours = shift.BreakHours
    };

    ShiftDto? response = await apiClient.PostAsync<CreateShiftRequestDto, ShiftDto>("Shifts", request, cancellationToken);

    return response is null ? null : MapToSummary(response);
  }

  public async Task<ShiftSummary?> UpdateShiftAsync(ShiftSummary shift, CancellationToken cancellationToken = default)
  {
    UpdateShiftRequestDto request = new()
    {
      ShiftId = shift.Id,
      Id = shift.Id,
      Name = shift.Name,
      ShiftStartTime = ParseTimeOrNull(shift.ShiftStartTime),
      ShiftBreakTime = ParseTimeOrNull(shift.ShiftBreakTime),
      ShiftBreakEndTime = ParseTimeOrNull(shift.ShiftBreakEndTime),
      ShiftEndTime = ParseTimeOrNull(shift.ShiftEndTime),
      BreakHours = shift.BreakHours
    };

    UpdateShiftResponseDto? response = await apiClient.PutAsync<UpdateShiftRequestDto, UpdateShiftResponseDto>(
      UpdateShiftRequestDto.BuildRoute(shift.Id),
      request,
      cancellationToken);

    return response?.Shift is null ? null : MapToSummary(response.Shift);
  }

  private static ShiftSummary MapToSummary(ShiftDto dto)
  {
    return new ShiftSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      ShiftStartTime = FormatTime(dto.ShiftStartTime),
      ShiftBreakTime = FormatTime(dto.ShiftBreakTime),
      ShiftBreakEndTime = FormatTime(dto.ShiftBreakEndTime),
      ShiftEndTime = FormatTime(dto.ShiftEndTime),
      BreakHours = dto.BreakHours
    };
  }

  private static string? FormatTime(TimeOnly? time)
  {
    return time?.ToString("HH:mm");
  }

  private static TimeOnly? ParseTimeOrNull(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return null;
    }

    return TimeOnly.TryParse(value, out TimeOnly parsed) ? parsed : null;
  }
}
