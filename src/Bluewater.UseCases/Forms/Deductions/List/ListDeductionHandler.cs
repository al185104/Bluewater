using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.UseCases.Deductions.List;
internal class ListDeductionHandler(IRepository<Deduction> _repository) : IQueryHandler<ListDeductionQuery, Result<IEnumerable<DeductionDTO>>>
{
  public async Task<Result<IEnumerable<DeductionDTO>>> Handle(ListDeductionQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new DeductionDTO(s.Id, s.EmployeeId, s.DeductionType, s.TotalAmount, s.MonthlyAmortization, s.RemainingBalance, s.NoOfMonths, s.StartDate, s.EndDate, s.Remarks, s.Status));
    return Result.Success(result);
  }
}
