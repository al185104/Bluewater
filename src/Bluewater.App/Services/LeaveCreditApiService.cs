using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class LeaveCreditApiService(IApiClient apiClient) : ILeaveCreditApiService
{
  public async Task<IReadOnlyList<LeaveCreditSummary>> GetLeaveCreditsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    LeaveCreditListResponseDto? response = await apiClient
      .GetAsync<LeaveCreditListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.LeaveCredits is not { Count: > 0 })
    {
      return Array.Empty<LeaveCreditSummary>();
    }

    return response.LeaveCredits
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
      return "LeaveCredits";
    }

    string query = string.Join('&', parameters);
    return $"LeaveCredits?{query}";
  }

  private static LeaveCreditSummary MapToSummary(LeaveCreditDto? dto)
  {
    if (dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new LeaveCreditSummary
    {
      Id = dto.Id,
      Code = dto.Code,
      Description = dto.Description,
      DefaultCredits = dto.DefaultCredits,
      SortOrder = dto.SortOrder,
      IsLeaveWithPay = dto.IsLeaveWithPay,
      IsCanCarryOver = dto.IsCanCarryOver
    };
  }
}
