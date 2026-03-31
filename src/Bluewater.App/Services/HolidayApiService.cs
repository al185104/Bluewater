using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class HolidayApiService(IApiClient apiClient) : IHolidayApiService
{
  public async Task<IReadOnlyList<HolidaySummary>> GetHolidaysAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    HolidayListResponseDto? response = await apiClient
      .GetAsync<HolidayListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Holidays is not { Count: > 0 })
    {
      return Array.Empty<HolidaySummary>();
    }

    return response.Holidays
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<HolidaySummary?> CreateHolidayAsync(HolidaySummary holiday, CancellationToken cancellationToken = default)
  {
    if (holiday is null)
    {
      throw new ArgumentNullException(nameof(holiday));
    }

    CreateHolidayRequestDto request = new()
    {
      Name = holiday.Name,
      Description = holiday.Description,
      Date = holiday.Date,
      IsRegular = holiday.IsRegular
    };

    HolidayDto? response = await apiClient
      .PostAsync<CreateHolidayRequestDto, HolidayDto>("Holidays", request, cancellationToken)
      .ConfigureAwait(false);

    return response is null ? null : MapToSummary(response);
  }

  public async Task<HolidaySummary?> UpdateHolidayAsync(HolidaySummary holiday, CancellationToken cancellationToken = default)
  {
    if (holiday is null)
    {
      throw new ArgumentNullException(nameof(holiday));
    }

    UpdateHolidayRequestDto request = new()
    {
      HolidayId = holiday.Id,
      Id = holiday.Id,
      Name = holiday.Name,
      Description = holiday.Description,
      Date = holiday.Date,
      IsRegular = holiday.IsRegular
    };

    UpdateHolidayResponseDto? response = await apiClient
      .PutAsync<UpdateHolidayRequestDto, UpdateHolidayResponseDto>(UpdateHolidayRequestDto.BuildRoute(holiday.Id), request, cancellationToken)
      .ConfigureAwait(false);

    return response?.Holiday is null ? null : MapToSummary(response.Holiday);
  }

  public Task<bool> DeleteHolidayAsync(Guid holidayId, CancellationToken cancellationToken = default)
  {
    if (holidayId == Guid.Empty)
    {
      throw new ArgumentException("Holiday ID must be provided", nameof(holidayId));
    }

    return apiClient.DeleteAsync($"Holidays/{holidayId}", cancellationToken);
  }

  private static string BuildRequestUri(int? skip, int? take)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    if (parameters.Count == 0)
    {
      return "Holidays";
    }

    string query = string.Join('&', parameters);
    return $"Holidays?{query}";
  }

  private static HolidaySummary MapToSummary(HolidayDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new HolidaySummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      Date = dto.Date,
      IsRegular = dto.IsRegular
    };
  }
}
