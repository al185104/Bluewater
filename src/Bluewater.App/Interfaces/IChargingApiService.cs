using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IChargingApiService
{
  Task<IReadOnlyList<ChargingSummary>> GetChargingsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<ChargingSummary?> GetChargingByIdAsync(Guid chargingId, CancellationToken cancellationToken = default);

  Task<ChargingSummary?> CreateChargingAsync(ChargingSummary charging, CancellationToken cancellationToken = default);

  Task<ChargingSummary?> UpdateChargingAsync(ChargingSummary charging, CancellationToken cancellationToken = default);

  Task<bool> DeleteChargingAsync(Guid chargingId, CancellationToken cancellationToken = default);
}
