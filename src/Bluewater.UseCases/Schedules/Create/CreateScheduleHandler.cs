using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;

namespace Bluewater.UseCases.Schedules.Create;

public class CreateScheduleHandler(IRepository<Schedule> _repository) : ICommandHandler<CreateScheduleCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
  {
    var newSchedule = new Schedule(request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault);
    var createdItem = await _repository.AddAsync(newSchedule, cancellationToken);
    return createdItem.Id;
  }
}
