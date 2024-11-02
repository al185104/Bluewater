using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.UseCases.Forms.OtherEarnings.Get;
public class GetOtherEarningHandler(IRepository<OtherEarning> _repository) : IQueryHandler<GetOtherEarningQuery, Result<OtherEarningDTO>>
{
  public async Task<Result<OtherEarningDTO>> Handle(GetOtherEarningQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.OtherEarningId, cancellationToken);
    if (result == null) return Result.NotFound();
    return new OtherEarningDTO(result.Id, result.EmployeeId, result.TotalAmount, result.IsActive, result.Date, result.Status);
  }
}
