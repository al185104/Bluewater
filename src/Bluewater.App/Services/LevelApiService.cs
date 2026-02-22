using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class LevelApiService(IApiClient apiClient) : ILevelApiService
{
  public async Task<IReadOnlyList<LevelSummary>> GetLevelsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    LevelListResponseDto? response = await apiClient
      .GetAsync<LevelListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Levels is not { Count: > 0 })
    {
      return Array.Empty<LevelSummary>();
    }

    return response.Levels
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<LevelSummary?> CreateLevelAsync(LevelSummary level, CancellationToken cancellationToken = default)
  {
    if (level is null)
    {
      throw new ArgumentNullException(nameof(level));
    }

    CreateLevelRequestDto request = new()
    {
      Name = level.Name,
      Value = level.Value,
      IsActive = level.IsActive
    };

    LevelDto? response = await apiClient
      .PostAsync<CreateLevelRequestDto, LevelDto>("Levels", request, cancellationToken)
      .ConfigureAwait(false);

    return response is null ? null : MapToSummary(response);
  }


  public async Task<LevelSummary?> UpdateLevelAsync(LevelSummary level, CancellationToken cancellationToken = default)
  {
    if (level is null)
    {
      throw new ArgumentNullException(nameof(level));
    }

    UpdateLevelRequestDto request = new()
    {
      LevelId = level.Id,
      Id = level.Id,
      Name = level.Name,
      Value = level.Value,
      IsActive = level.IsActive
    };

    UpdateLevelResponseDto? response = await apiClient
      .PutAsync<UpdateLevelRequestDto, UpdateLevelResponseDto>(UpdateLevelRequestDto.BuildRoute(level.Id), request, cancellationToken)
      .ConfigureAwait(false);

    return response?.Level is null ? null : MapToSummary(response.Level);
  }

  public Task<bool> DeleteLevelAsync(Guid levelId, CancellationToken cancellationToken = default)
  {
    if (levelId == Guid.Empty)
    {
      throw new ArgumentException("Level ID must be provided", nameof(levelId));
    }

    return apiClient.DeleteAsync($"Levels/{levelId}", cancellationToken);
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
      return "Levels";
    }

    string query = string.Join('&', parameters);
    return $"Levels?{query}";
  }

  private static LevelSummary MapToSummary(LevelDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new LevelSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Value = dto.Value,
      IsActive = dto.IsActive
    };
  }
}
