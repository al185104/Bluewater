using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.FailureInOutAggregate.Specifications;
using Bluewater.Core.Forms.FailureInOutAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts.List;
internal class ListFailureInOutHandler(IRepository<FailureInOut> _repository) : IQueryHandler<ListFailureInOutQuery, Result<IEnumerable<FailureInOutDTO>>>
{
  public async Task<Result<IEnumerable<FailureInOutDTO>>> Handle(ListFailureInOutQuery request, CancellationToken cancellationToken)
  {
    var spec = new FailureInOutAllSpec(request.tenant);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new FailureInOutDTO(s.Id, s.EmployeeId, $"{s.Employee?.LastName}, {s.Employee?.FirstName}", s.Date, s.Remarks, (FailureInOutReasonDTO?)s.Reason, (ApplicationStatusDTO)s.Status)));
  }
}
