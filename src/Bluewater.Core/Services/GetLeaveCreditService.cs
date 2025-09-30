using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Services;

public class GetLeaveCreditService(IReadRepository<LeaveCredit> _repository) : IGetLeaveCreditService
{
  public async Task<LeaveCredit?> GetLeaveCreditAsync(Guid leaveCreditId, CancellationToken cancellationToken = default)
  {
    return await _repository.GetByIdAsync(leaveCreditId, cancellationToken);
  }
}
