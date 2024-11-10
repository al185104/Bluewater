using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.DeductionAggregate.Specifications;
using Bluewater.Core.Forms.DeductionAggregate;
using Bluewater.Core.Forms.Enum;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.List;
internal class ListDeductionByEmpIdDatesHandler(IRepository<Deduction> _repository) : IQueryHandler<ListDeductionByEmpIdDatesQuery, Result<IEnumerable<DeductionDTO>>>
{
  public async Task<Result<IEnumerable<DeductionDTO>>> Handle(ListDeductionByEmpIdDatesQuery request, CancellationToken cancellationToken)
  {
    var spec = new DeductionByEmpIdDatesSpec(request.empId, (ApplicationStatus?)request.status, request.end);
    var result = await _repository.ListAsync(spec, cancellationToken);
    return Result.Success(result.Select(s => new DeductionDTO(s.Id, s.EmployeeId, $"{s.Employee?.LastName}, {s.Employee?.FirstName}", (DeductionsTypeDTO)s.DeductionType, s.TotalAmount, s.MonthlyAmortization, s.RemainingBalance, s.NoOfMonths, s.StartDate, s.EndDate, s.Remarks, (ApplicationStatusDTO)s.Status)));
  }
}

