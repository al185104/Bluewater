using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class ChargingApiService(IApiClient apiClient) : IChargingApiService
{
  public async Task<IReadOnlyList<ChargingSummary>> GetChargingsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    ChargingListResponseDto? response = await apiClient.GetAsync<ChargingListResponseDto>(requestUri, cancellationToken);

    if (response?.Chargings is not { Count: > 0 })
    {
      return Array.Empty<ChargingSummary>();
    }

    return response.Chargings
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<ChargingSummary?> GetChargingByIdAsync(Guid chargingId, CancellationToken cancellationToken = default)
  {
    if (chargingId == Guid.Empty)
    {
      throw new ArgumentException("Charging ID must be provided", nameof(chargingId));
    }

    ChargingDto? dto = await apiClient.GetAsync<ChargingDto>($"Chargings/{chargingId}", cancellationToken);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<ChargingSummary?> CreateChargingAsync(ChargingSummary charging, CancellationToken cancellationToken = default)
  {
    if (charging is null)
    {
      throw new ArgumentNullException(nameof(charging));
    }

    CreateChargingRequestDto request = new()
    {
      Name = charging.Name,
      Description = charging.Description,
      DepartmentId = charging.DepartmentId
    };

    ChargingDto? response = await apiClient.PostAsync<CreateChargingRequestDto, ChargingDto>(CreateChargingRequestDto.Route, request, cancellationToken);
    return response is null ? null : MapToSummary(response);
  }

  public async Task<ChargingSummary?> UpdateChargingAsync(ChargingSummary charging, CancellationToken cancellationToken = default)
  {
    if (charging is null)
    {
      throw new ArgumentNullException(nameof(charging));
    }

    if (charging.Id == Guid.Empty)
    {
      throw new ArgumentException("Charging ID must be provided", nameof(charging));
    }

    UpdateChargingRequestDto request = new()
    {
      Id = charging.Id,
      Name = charging.Name,
      Description = charging.Description,
      DepartmentId = charging.DepartmentId
    };

    UpdateChargingResponseDto? response = await apiClient.PutAsync<UpdateChargingRequestDto, UpdateChargingResponseDto>(
      UpdateChargingRequestDto.Route,
      request,
      cancellationToken);

    return response?.Charging is null ? null : MapToSummary(response.Charging);
  }

  public Task<bool> DeleteChargingAsync(Guid chargingId, CancellationToken cancellationToken = default)
  {
    if (chargingId == Guid.Empty)
    {
      throw new ArgumentException("Charging ID must be provided", nameof(chargingId));
    }

    return apiClient.DeleteAsync($"Chargings/{chargingId}", cancellationToken);
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
      return "Chargings";
    }

    string query = string.Join('&', parameters);
    return $"Chargings?{query}";
  }

  private static ChargingSummary MapToSummary(ChargingDto dto)
  {
    return new ChargingSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      DepartmentId = dto.DepartmentId
    };
  }
}
