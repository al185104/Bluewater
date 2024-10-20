using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.Core.ShiftAggregate;
using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules.Get;
public class GetDefaultScheduleHandler(IRepository<Schedule> _schedRepository, IRepository<Shift> _shiftRepository) : IQueryHandler<GetDefaultScheduleQuery, Result<IEnumerable<ShiftDTO>>>
{
  public async Task<Result<IEnumerable<ShiftDTO>>> Handle(GetDefaultScheduleQuery request, CancellationToken cancellationToken)
  {
    var spec = new DefaultShiftByEmpIdSpec(request.empId);
    var defaultSched = await _schedRepository.ListAsync(spec, cancellationToken);

    // loop through the week and if something is missing, add a default shift
    foreach (var dayOfTheWeek in Enum.GetValues<DayOfWeek>())
    {
      if (defaultSched.Any(s => s.ScheduleDate.DayOfWeek == dayOfTheWeek)) continue;
      
      // get the date of the current day of the week this week.
      var today = DateTime.Today;
      var dayDiff = (int)dayOfTheWeek - (int)today.DayOfWeek;
      if (dayDiff < 0) dayDiff += 7;
      var dayOfTheWeekDate = today.AddDays(dayDiff);

      var defaultShift = new Schedule(request.empId, Guid.Empty, DateOnly.FromDateTime(dayOfTheWeekDate), true);
      defaultSched.Add(defaultShift);
    }

    List<ShiftDTO> shifts = new();
    foreach (var sched in defaultSched)
    {
      var shiftResult = await _shiftRepository.GetByIdAsync(sched.ShiftId, cancellationToken);
      if (shiftResult != null)
        shifts.Add(new ShiftDTO(shiftResult!.Id, shiftResult.Name, shiftResult.ShiftStartTime, shiftResult.ShiftBreakTime, shiftResult.ShiftBreakEndTime, shiftResult.ShiftEndTime, shiftResult.BreakHours));
      else
        shifts.Add(new ShiftDTO(Guid.Empty, "No Shift", null, null, null, null, 0));
    }
    
    return shifts;
  }
}
