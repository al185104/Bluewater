using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class DivisionApiService(IApiClient apiClient) : IDivisionApiService
{
  public async Task<IReadOnlyList<DivisionSummary>> GetDivisionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    DivisionListResponseDto? response = await apiClient
      .GetAsync<DivisionListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Divisions is not { Count: > 0 })
    {
      return Array.Empty<DivisionSummary>();
    }

    return response.Divisions
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<DivisionSummary?> GetDivisionByIdAsync(Guid divisionId, CancellationToken cancellationToken = default)
  {
    if (divisionId == Guid.Empty)
    {
      throw new ArgumentException("Division ID must be provided", nameof(divisionId));
    }

    DivisionDto? dto = await apiClient
      .GetAsync<DivisionDto>($"Divisions/{divisionId}", cancellationToken)
      .ConfigureAwait(false);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<DivisionSummary?> CreateDivisionAsync(DivisionSummary division, CancellationToken cancellationToken = default)
  {
    if (division is null)
    {
      throw new ArgumentNullException(nameof(division));
    }

    CreateDivisionRequestDto request = new()
    {
      Name = division.Name,
      Description = division.Description
    };

    DivisionDto? response = await apiClient
      .PostAsync<CreateDivisionRequestDto, DivisionDto>("Divisions", request, cancellationToken)
      .ConfigureAwait(false);
    return response is null ? null : MapToSummary(response);
  }

  public async Task<DivisionSummary?> UpdateDivisionAsync(DivisionSummary division, CancellationToken cancellationToken = default)
  {
    if (division is null)
    {
      throw new ArgumentNullException(nameof(division));
    }

    if (division.Id == Guid.Empty)
    {
      throw new ArgumentException("Division ID must be provided", nameof(division));
    }

    UpdateDivisionRequestDto request = new()
    {
      DivisionId = division.Id,
      Id = division.Id,
      Name = division.Name,
      Description = division.Description
    };

    UpdateDivisionResponseDto? response = await apiClient
      .PutAsync<UpdateDivisionRequestDto, UpdateDivisionResponseDto>(
      UpdateDivisionRequestDto.BuildRoute(division.Id),
      request,
      cancellationToken)
      .ConfigureAwait(false);

    return response?.Division is null ? null : MapToSummary(response.Division);
  }

  public Task<bool> DeleteDivisionAsync(Guid divisionId, CancellationToken cancellationToken = default)
  {
    if (divisionId == Guid.Empty)
    {
      throw new ArgumentException("Division ID must be provided", nameof(divisionId));
    }

    return apiClient.DeleteAsync($"Divisions/{divisionId}", cancellationToken);
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
      return "Divisions";
    }

    string query = string.Join('&', parameters);
    return $"Divisions?{query}";
  }

  private static DivisionSummary MapToSummary(DivisionDto? dto)
  {
    if(dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new DivisionSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description
    };
  }
}
