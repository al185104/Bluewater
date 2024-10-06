using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ShiftAggregate;

namespace Bluewater.UseCases.Shifts.Update;
public class UpdateShiftHandler(IRepository<Shift> _repository) : ICommandHandler<UpdateShiftCommand, Result<ShiftDTO>>
{
  public async Task<Result<ShiftDTO>> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
  {
    var existingShift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
    if (existingShift == null)
    {
      return Result.NotFound();
    }

    existingShift.UpdateShift(request.NewName, request.start, request.breakstart, request.breakend, request.end, request.breakhours);

    await _repository.UpdateAsync(existingShift, cancellationToken);

    return Result.Success(new ShiftDTO(existingShift.Id, existingShift.Name, existingShift.ShiftStartTime, existingShift.ShiftBreakTime, existingShift.ShiftBreakEndTime, existingShift.ShiftEndTime, existingShift.BreakHours));
  }
}
