using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IPositionApiService
{
  Task<IReadOnlyList<PositionSummary>> GetPositionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<PositionSummary?> GetPositionByIdAsync(Guid positionId, CancellationToken cancellationToken = default);

  Task<PositionSummary?> CreatePositionAsync(PositionSummary position, CancellationToken cancellationToken = default);

  Task<PositionSummary?> UpdatePositionAsync(PositionSummary position, CancellationToken cancellationToken = default);

  Task<bool> DeletePositionAsync(Guid positionId, CancellationToken cancellationToken = default);
}
