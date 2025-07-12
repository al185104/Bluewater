using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.Forms.LeaveAggregate.Specifications;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.List;

public class ListLeaveHandler(IRepository<Leave> _repository) : IQueryHandler<ListLeaveQuery, Result<IEnumerable<LeaveDTO>>>
{
  public async Task<Result<IEnumerable<LeaveDTO>>> Handle(ListLeaveQuery request, CancellationToken cancellationToken)
  {
    var spec = new LeaveAllSpec(request.tenant);
    var leaves = await _repository.ListAsync(spec, cancellationToken);
    if (leaves == null) return Result.NotFound();

    var result = leaves.Select(s => new LeaveDTO(s.Id, s.StartDate, s.EndDate, s.IsHalfDay, (ApplicationStatusDTO)s.Status, s.EmployeeId ?? Guid.Empty, s.LeaveCreditId, $"{s.Employee?.LastName}, {s.Employee?.FirstName}", $"{s.LeaveCredit?.LeaveCode}"));
    return Result.Success(result);
  }
}
