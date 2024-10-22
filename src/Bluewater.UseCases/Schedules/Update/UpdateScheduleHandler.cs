using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;

namespace Bluewater.UseCases.Schedules.Update;
public class UpdateScheduleHandler(IRepository<Schedule> _schedRepository, IRepository<Employee> _empRepository) : ICommandHandler<UpdateScheduleCommand, Result<ScheduleDTO>>
{
  public async Task<Result<ScheduleDTO>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
  {
    var spec = new ScheduleByIdSpec(request.ScheduleId);
    var existingSchedule = await _schedRepository.FirstOrDefaultAsync(spec, cancellationToken);
    if (existingSchedule == null) return Result.NotFound();

    existingSchedule.UpdateSchedule(request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault);

    await _schedRepository.UpdateAsync(existingSchedule, cancellationToken);

    var emp = await _empRepository.GetByIdAsync(existingSchedule.EmployeeId, cancellationToken);
    if (emp == null) return Result.NotFound();

    return Result.Success(new ScheduleDTO(existingSchedule.Id, $"{emp.LastName}, {emp.FirstName}", existingSchedule.EmployeeId, existingSchedule.ShiftId, existingSchedule.ScheduleDate, existingSchedule.IsDefault));
  }
}
