using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class PositionApiService(IApiClient apiClient) : IPositionApiService
{
  public async Task<IReadOnlyList<PositionSummary>> GetPositionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    PositionListResponseDto? response = await apiClient.GetAsync<PositionListResponseDto>(requestUri, cancellationToken);

    if (response?.Positions is not { Count: > 0 })
    {
      return Array.Empty<PositionSummary>();
    }

    return response.Positions
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<PositionSummary?> GetPositionByIdAsync(Guid positionId, CancellationToken cancellationToken = default)
  {
    if (positionId == Guid.Empty)
    {
      throw new ArgumentException("Position ID must be provided", nameof(positionId));
    }

    PositionDto? dto = await apiClient.GetAsync<PositionDto>($"Positions/{positionId}", cancellationToken);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<PositionSummary?> CreatePositionAsync(PositionSummary position, CancellationToken cancellationToken = default)
  {
    if (position is null)
    {
      throw new ArgumentNullException(nameof(position));
    }

    CreatePositionRequestDto request = new()
    {
      Name = position.Name,
      Description = position.Description,
      SectionId = position.SectionId
    };

    PositionDto? response = await apiClient.PostAsync<CreatePositionRequestDto, PositionDto>("Positions", request, cancellationToken);
    return response is null ? null : MapToSummary(response);
  }

  public async Task<PositionSummary?> UpdatePositionAsync(PositionSummary position, CancellationToken cancellationToken = default)
  {
    if (position is null)
    {
      throw new ArgumentNullException(nameof(position));
    }

    if (position.Id == Guid.Empty)
    {
      throw new ArgumentException("Position ID must be provided", nameof(position));
    }

    UpdatePositionRequestDto request = new()
    {
      PositionId = position.Id,
      Id = position.Id,
      Name = position.Name,
      Description = position.Description,
      SectionId = position.SectionId
    };

    UpdatePositionResponseDto? response = await apiClient.PutAsync<UpdatePositionRequestDto, UpdatePositionResponseDto>(
      UpdatePositionRequestDto.BuildRoute(position.Id),
      request,
      cancellationToken);

    return response?.Position is null ? null : MapToSummary(response.Position);
  }

  public Task<bool> DeletePositionAsync(Guid positionId, CancellationToken cancellationToken = default)
  {
    if (positionId == Guid.Empty)
    {
      throw new ArgumentException("Position ID must be provided", nameof(positionId));
    }

    return apiClient.DeleteAsync($"Positions/{positionId}", cancellationToken);
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
      return "Positions";
    }

    string query = string.Join('&', parameters);
    return $"Positions?{query}";
  }

  private static PositionSummary MapToSummary(PositionDto dto)
  {
    return new PositionSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      SectionId = dto.SectionId
    };
  }
}
