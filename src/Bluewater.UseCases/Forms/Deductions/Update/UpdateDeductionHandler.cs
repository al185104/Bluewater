using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.UseCases.Deductions.Update;
public class UpdateDeductionHandler(IRepository<Deduction> _repository) : ICommandHandler<UpdateDeductionCommand, Result<DeductionDTO>>
{
  public async Task<Result<DeductionDTO>> Handle(UpdateDeductionCommand request, CancellationToken cancellationToken)
  {
    var existingDeduction = await _repository.GetByIdAsync(request.id, cancellationToken);
    if (existingDeduction == null)
    {
      return Result.NotFound();
    }

    existingDeduction.UpdateDeduction(request.empId, request.type, request.totalAmount, request.monthlyAmortization, request.remainingBalance, request.noOfMonths, request.startDate, request.endDate, request.remarks, request.status);

    await _repository.UpdateAsync(existingDeduction, cancellationToken);

    return Result.Success(new DeductionDTO(existingDeduction.Id, existingDeduction.EmployeeId, existingDeduction.DeductionType, existingDeduction.TotalAmount, existingDeduction.MonthlyAmortization, existingDeduction.RemainingBalance, existingDeduction.NoOfMonths, existingDeduction.StartDate, existingDeduction.EndDate, existingDeduction.Remarks, existingDeduction.Status));
  }
}
