using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.List;

public class ListLeaveHandler(IRepository<Leave> _repository) : IQueryHandler<ListLeaveQuery, Result<IEnumerable<LeaveDTO>>>
{
  public async Task<Result<IEnumerable<LeaveDTO>>> Handle(ListLeaveQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new LeaveDTO(s.Id, s.StartDate, s.EndDate, s.IsHalfDay, (ApplicationStatusDTO)s.Status, s.EmployeeId ?? Guid.Empty, s.LeaveCreditId));
    return Result.Success(result);
  }
}
