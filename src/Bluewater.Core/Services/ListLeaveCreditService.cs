using System.Linq;
using Ardalis.SharedKernel;
using Bluewater.Core.Interfaces;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Services;

public class ListLeaveCreditService(IReadRepository<LeaveCredit> _repository) : IListLeaveCreditService
{
  public async Task<IEnumerable<LeaveCredit>> ListLeaveCreditsAsync(CancellationToken cancellationToken = default)
  {
    var leaveCredits = await _repository.ListAsync(cancellationToken);
    return leaveCredits.AsEnumerable();
  }
}
