using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.FailureInOutAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts.List;
internal class ListFailureInOutHandler(IRepository<FailureInOut> _repository) : IQueryHandler<ListFailureInOutQuery, Result<IEnumerable<FailureInOutDTO>>>
{
  public async Task<Result<IEnumerable<FailureInOutDTO>>> Handle(ListFailureInOutQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new FailureInOutDTO(s.Id, s.EmployeeId, s.Date, s.Remarks, (FailureInOutReasonDTO?)s.Reason, (ApplicationStatusDTO?)s.Status));
    return Result.Success(result);
  }
}
