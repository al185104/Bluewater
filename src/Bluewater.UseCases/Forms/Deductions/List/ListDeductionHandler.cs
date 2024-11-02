using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DeductionAggregate.Specifications;
using Bluewater.Core.Forms.DeductionAggregate;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.List;
internal class ListDeductionHandler(IRepository<Deduction> _repository) : IQueryHandler<ListDeductionQuery, Result<IEnumerable<DeductionDTO>>>
{
  public async Task<Result<IEnumerable<DeductionDTO>>> Handle(ListDeductionQuery request, CancellationToken cancellationToken)
  {
    var spec = new DeductionAllSpec();
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new DeductionDTO(s.Id, s.EmployeeId, $"{s.Employee?.LastName}, {s.Employee?.FirstName}", (DeductionsTypeDTO)s.DeductionType, s.TotalAmount, s.MonthlyAmortization, s.RemainingBalance, s.NoOfMonths, s.StartDate, s.EndDate, s.Remarks, (ApplicationStatusDTO)s.Status)));
  }
}
