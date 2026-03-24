using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IDeductionApiService
{
  Task<IReadOnlyList<DeductionSummary>> GetDeductionsAsync(
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);

  Task<DeductionSummary?> CreateDeductionAsync(DeductionSummary deduction, CancellationToken cancellationToken = default);
  Task<DeductionSummary?> UpdateDeductionAsync(DeductionSummary deduction, CancellationToken cancellationToken = default);
}
