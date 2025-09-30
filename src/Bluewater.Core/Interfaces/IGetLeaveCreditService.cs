using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.Core.Interfaces;

public interface IGetLeaveCreditService
{
  Task<LeaveCredit?> GetLeaveCreditAsync(Guid leaveCreditId, CancellationToken cancellationToken = default);
}
