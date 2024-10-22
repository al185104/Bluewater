using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.UseCases.Shifts;

namespace Bluewater.UseCases.Schedules.Get;
public class GetDefaultScheduleHandler(IRepository<Schedule> _schedRepository) : IQueryHandler<GetDefaultScheduleQuery, Result<IEnumerable<ScheduleDTO>>>
{
  public async Task<Result<IEnumerable<ScheduleDTO>>> Handle(GetDefaultScheduleQuery request, CancellationToken cancellationToken)
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

      var defaultShift = new Schedule(request.empId, Guid.Empty, DateOnly.FromDateTime(dayOfTheWeekDate), false);
      defaultSched.Add(defaultShift);
    }
    
    // get the name based on the first none null Shift
    return Result.Success(defaultSched.Select(s => new ScheduleDTO(s.Id, string.Empty, s.EmployeeId, s.ShiftId, s.ScheduleDate, s.IsDefault, 
    s.Shift == null ? null : new ShiftDTO(s.Shift.Id, s.Shift.Name, s.Shift.ShiftStartTime, s.Shift.ShiftBreakTime, s.Shift.ShiftBreakEndTime, s.Shift.ShiftEndTime, s.Shift.BreakHours))));
  }
}
