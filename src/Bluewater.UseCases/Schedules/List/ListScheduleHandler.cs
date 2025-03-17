using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using Bluewater.UseCases.Shifts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Schedules.List;

internal class ListScheduleHandler(IRepository<Schedule> _schedRepository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListScheduleQuery, Result<IEnumerable<EmployeeScheduleDTO>>>
{
  public async Task<Result<IEnumerable<EmployeeScheduleDTO>>> Handle(ListScheduleQuery request, CancellationToken cancellationToken)
  {
    List<EmployeeDTO> employees = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListEmployeeQuery(null, null));
        if (ret.IsSuccess)
            employees = ret.Value.Where(i => i.Charging != null && i.Charging.Equals(request.chargingName, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    List<EmployeeScheduleDTO> results = new();
    // get all schedules per employee and per date. If no schedule, create a default schedule
    foreach (var emp in employees)
    {
        var spec = new ScheduleByEmpIdAndDatesSpec(emp.Id, request.startDate, request.endDate);
        var scheds = await _schedRepository.ListAsync(spec, cancellationToken);
        if(scheds == null) continue;

        // loop for dates and create default schedule if not exists
        List<ShiftInfo> shifts = new();
        for (var date = request.startDate; date <= request.endDate; date = date.AddDays(1))
        {
            // check if there's a schedule for the date
            var sched = scheds!.FirstOrDefault(s => s.ScheduleDate == date);
            // if no schedule, check first if there's a default schedule
            if(sched == null) {
                var defaultSpec = new DefaultShiftByEmpIdSpec(emp.Id); //date.DayOfWeek
                var list = await _schedRepository.ListAsync(defaultSpec, cancellationToken);

                var defaultSched = list.FirstOrDefault(i => i.ScheduleDate.DayOfWeek == date.DayOfWeek);

                // if no default schedule, create a default schedule
                if(defaultSched == null) {
                    var defaultShift = new ShiftDTO(Guid.Empty, string.Empty, null, null, null, null, 0);
                    shifts.Add(new ShiftInfo(){
                        ScheduleId = Guid.Empty,
                        Shift = defaultShift,
                        ScheduleDate = date,
                        IsDefault = false
                    });
                } 
                else 
                {
                    var shift = new ShiftDTO(defaultSched.Shift.Id, defaultSched.Shift.Name, defaultSched.Shift.ShiftStartTime, defaultSched.Shift.ShiftBreakTime, defaultSched.Shift.ShiftBreakEndTime, defaultSched.Shift.ShiftEndTime, defaultSched.Shift.BreakHours);
                    shifts.Add(new ShiftInfo(){
                        ScheduleId = defaultSched.Id,
                        Shift = shift,
                        ScheduleDate = date,
                        IsDefault = defaultSched.IsDefault
                    });
                }
            } 
            else 
            {
                var shift = new ShiftDTO(sched.Shift!.Id, sched.Shift.Name, sched.Shift.ShiftStartTime, sched.Shift.ShiftBreakTime, sched.Shift.ShiftBreakEndTime, sched.Shift.ShiftEndTime, sched.Shift.BreakHours);
                shifts.Add(new ShiftInfo(){
                    ScheduleId = sched.Id,
                    Shift = shift,
                    ScheduleDate = date,
                    IsDefault = sched.IsDefault
                });
            }   
        }
        results.Add(new EmployeeScheduleDTO(emp.Id, emp.User!.Username, $"{emp.LastName}, {emp.FirstName}", emp.Section ?? string.Empty, emp.Charging ?? string.Empty, shifts.OrderBy(s => s.ScheduleDate).ToList()));
    }

    return results.OrderBy(r => r.Name).ToList();
  }
}
