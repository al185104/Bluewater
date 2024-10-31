using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.UseCases.Deductions.Create;
public class CreateDeductionHandler(IRepository<Deduction> _repository) : ICommandHandler<CreateDeductionCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDeductionCommand request, CancellationToken cancellationToken)
  {
    var newDeduction = new Deduction(request.empId, request.type, request.totalAmount, request.monthlyAmortization, request.remainingBalance, request.noOfMonths, request.startDate, request.endDate, request.remarks);
    var createdItem = await _repository.AddAsync(newDeduction, cancellationToken);
    return createdItem.Id;
  }
}
