using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Shifts.Create;

public class CreateShiftHandler(IRepository<Shift> _repository) : ICommandHandler<CreateShiftCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
  {
    var existingShift = await _repository.FirstOrDefaultAsync(new ShiftByNameSpec(request.Name), cancellationToken);
    if (existingShift != null)
    {
      return existingShift.Id;
    }

    var newShift = new Shift(request.Name, request.ShiftStartTime, request.ShiftBreakTime, request.ShiftBreakEndTime, request.ShiftEndTime, request.BreakHours);
    var createdItem = await _repository.AddAsync(newShift, cancellationToken);
    return createdItem.Id;
  }
}
