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

    LevelListResponseDto? response = await apiClient.GetAsync<LevelListResponseDto>(requestUri, cancellationToken);

    if (response?.Levels is not { Count: > 0 })
    {
      return Array.Empty<LevelSummary>();
    }

    return response.Levels
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
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
