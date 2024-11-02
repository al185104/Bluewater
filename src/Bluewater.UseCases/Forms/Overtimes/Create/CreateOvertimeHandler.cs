using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;

namespace Bluewater.UseCases.Forms.Overtimes.Create;
public class CreateOvertimeHandler(IRepository<Overtime> _repository) : ICommandHandler<CreateOvertimeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateOvertimeCommand request, CancellationToken cancellationToken)
  {
    var newOvertime = new Overtime(request.empId, request.startDate, request.endDate, request.approvedHours, request.remarks);
    var createdItem = await _repository.AddAsync(newOvertime, cancellationToken);
    return createdItem.Id;
  }
}
