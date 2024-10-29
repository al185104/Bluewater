using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;

namespace Bluewater.UseCases.Schedules.Get;
public class GetDefaultScheduleByEmpIdAndDayHandler(IRepository<Schedule> _schedRepository, IRepository<Employee> _empRepository) : IQueryHandler<GetDefaultScheduleByEmpIdAndDayQuery, Result<ScheduleDTO>>
{
  public async Task<Result<ScheduleDTO>> Handle(GetDefaultScheduleByEmpIdAndDayQuery request, CancellationToken cancellationToken)
  {
    var spec = new DefaultShiftByEmpIdSpec(request.empId, request.dayOfWeek);
    var schedResult = await _schedRepository.FirstOrDefaultAsync(spec, cancellationToken);
    if (schedResult == null || schedResult!.EmployeeId == Guid.Empty) return Result.NotFound();

    var empResult = await _empRepository.GetByIdAsync(schedResult!.EmployeeId, cancellationToken);
    if (empResult == null) return Result.NotFound();
    
    return new ScheduleDTO(schedResult!.Id, $"{empResult!.LastName}, {empResult!.FirstName}", schedResult.EmployeeId, schedResult.ShiftId, schedResult.ScheduleDate, schedResult.IsDefault,
    new Shifts.ShiftDTO(schedResult.Shift.Id, schedResult.Shift.Name, schedResult.Shift.ShiftStartTime, schedResult.Shift.ShiftBreakTime, schedResult.Shift.ShiftBreakEndTime, schedResult.Shift.ShiftEndTime, schedResult.Shift.BreakHours));
  }
}
