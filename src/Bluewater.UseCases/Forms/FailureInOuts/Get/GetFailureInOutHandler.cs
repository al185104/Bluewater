using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.FailureInOutAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.FailureInOuts.Get;
public class GetFailureInOutHandler(IRepository<FailureInOut> _repository) : IQueryHandler<GetFailureInOutQuery, Result<FailureInOutDTO>>
{
  public async Task<Result<FailureInOutDTO>> Handle(GetFailureInOutQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.FailureInOutId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new FailureInOutDTO(result.Id, result.EmployeeId, result.Date, result.Remarks, (FailureInOutReasonDTO?)result.Reason, (ApplicationStatusDTO?)result.Status);
  }
}
