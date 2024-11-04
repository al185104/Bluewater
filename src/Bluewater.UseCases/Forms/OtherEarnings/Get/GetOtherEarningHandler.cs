using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Get;
public class GetOtherEarningHandler(IRepository<OtherEarning> _repository) : IQueryHandler<GetOtherEarningQuery, Result<OtherEarningDTO>>
{
  public async Task<Result<OtherEarningDTO>> Handle(GetOtherEarningQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.OtherEarningId, cancellationToken);
    if (result == null) return Result.NotFound();
    return new OtherEarningDTO(result.Id, result.EmployeeId, $"{result.Employee!.LastName}, {result.Employee!.FirstName}", (OtherEarningTypeDTO?)result.EarningType ?? default, result.TotalAmount, result.IsActive, result.Date, (ApplicationStatusDTO)result.Status);
  }
}
