using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;

namespace Bluewater.UseCases.Schedules.Create;

public class CreateScheduleHandler(IRepository<Schedule> _repository) : ICommandHandler<CreateScheduleCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
  {
    // check first if the schedule already exists, if it does, update it.
    var existingSchedule = await _repository.FirstOrDefaultAsync(new ScheduleByEmpIdAndDateSpec(request.EmployeeId, request.ScheduleDate), cancellationToken);
    if (existingSchedule != null)
    {
      existingSchedule.UpdateSchedule(request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault);
      await _repository.UpdateAsync(existingSchedule, cancellationToken);
      return existingSchedule.Id;
    }

    var newSchedule = new Schedule(request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault);
    var createdItem = await _repository.AddAsync(newSchedule, cancellationToken);
    return createdItem.Id;
  }
}
