using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Undertimes.Get;
public class GetUndertimeHandler(IRepository<Undertime> _repository) : IQueryHandler<GetUndertimeQuery, Result<UndertimeDTO>>
{
  public async Task<Result<UndertimeDTO>> Handle(GetUndertimeQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.UndertimeId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new UndertimeDTO(result.Id, result.EmployeeId, $"{result.Employee!.LastName}, {result.Employee!.FirstName}", result.InclusiveTime, result.Reason, result.Date, (ApplicationStatusDTO)result.Status);
  }
}
