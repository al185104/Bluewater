
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.LeaveCreditAggregate;

namespace Bluewater.UseCases.LeaveCredits.List;

public class ListLeaveCreditHandler(IRepository<LeaveCredit> _repository) : IQueryHandler<ListLeaveCreditQuery, Result<IEnumerable<LeaveCreditDTO>>>
{
  public async Task<Result<IEnumerable<LeaveCreditDTO>>> Handle(ListLeaveCreditQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new LeaveCreditDTO(s.Id, s.LeaveCode, s.LeaveDescription, s.DefaultCredits));
    return Result.Success(result);
  }
}
