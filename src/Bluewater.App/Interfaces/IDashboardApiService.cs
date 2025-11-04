using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IDashboardApiService
{
  Task<HomeDashboardSummary?> GetHomeDashboardAsync(
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default);
}
