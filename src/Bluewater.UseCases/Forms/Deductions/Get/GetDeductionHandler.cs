using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.DeductionAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.Get;
public class GetDeductionHandler(IRepository<Deduction> _repository) : IQueryHandler<GetDeductionQuery, Result<DeductionDTO>>
{
  public async Task<Result<DeductionDTO>> Handle(GetDeductionQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.DeductionId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new DeductionDTO(result.Id, result.EmployeeId, $"{result.Employee?.LastName}, {result.Employee?.FirstName}", (DeductionsTypeDTO)result.DeductionType, result.TotalAmount, result.MonthlyAmortization, result.RemainingBalance, result.NoOfMonths, result.StartDate, result.EndDate, result.Remarks, (ApplicationStatusDTO)result.Status);
  }
}
