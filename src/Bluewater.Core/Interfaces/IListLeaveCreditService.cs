using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Interfaces;

public interface IListLeaveCreditService
{
  Task<IEnumerable<LeaveCredit>> ListLeaveCreditsAsync(CancellationToken cancellationToken = default);
}
