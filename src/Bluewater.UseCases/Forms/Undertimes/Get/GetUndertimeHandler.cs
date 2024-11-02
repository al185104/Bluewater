using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.UseCases.Forms.Undertimes.Get;
public class GetUndertimeHandler(IRepository<Undertime> _repository) : IQueryHandler<GetUndertimeQuery, Result<UndertimeDTO>>
{
  public async Task<Result<UndertimeDTO>> Handle(GetUndertimeQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.UndertimeId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new UndertimeDTO(result.Id, result.EmployeeId, result.InclusiveTime, result.Reason, result.Date, result.Status);
  }
}
