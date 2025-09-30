using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IDivisionApiService
{
  Task<IReadOnlyList<DivisionSummary>> GetDivisionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<DivisionSummary?> GetDivisionByIdAsync(Guid divisionId, CancellationToken cancellationToken = default);

  Task<DivisionSummary?> CreateDivisionAsync(DivisionSummary division, CancellationToken cancellationToken = default);

  Task<DivisionSummary?> UpdateDivisionAsync(DivisionSummary division, CancellationToken cancellationToken = default);

  Task<bool> DeleteDivisionAsync(Guid divisionId, CancellationToken cancellationToken = default);
}
