using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IServiceChargeApiService
{
  Task<IReadOnlyList<ServiceChargeSummary>> GetServiceChargesByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

  Task<Guid?> CreateServiceChargeAsync(ServiceChargeSummary serviceCharge, CancellationToken cancellationToken = default);
}
