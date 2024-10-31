using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Schedules;
using Bluewater.UseCases.Schedules.Get;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.Get;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Attendances.List;

internal class ListAttendanceHandler(IRepository<Attendance> _repository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAttendanceQuery, Result<IEnumerable<AttendanceDTO>>>
{
  public async Task<Result<IEnumerable<AttendanceDTO>>> Handle(ListAttendanceQuery request, CancellationToken cancellationToken)
  {
    var spec = new AttendanceByEmpIdAndDatesSpec(request.empId, request.startDate, request.endDate);
    var attendances = await _repository.ListAsync(spec, cancellationToken);
    if(attendances == null) return Result<IEnumerable<AttendanceDTO>>.NotFound();

    // get employee by employee id
    EmployeeDTO emp = default!;
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new GetEmployeeQuery(request.empId));
        if (ret.IsSuccess)
            emp = ret.Value;
    }

    List<AttendanceDTO> results = new();
    for (var date = request.startDate; date <= request.endDate; date = date.AddDays(1))
    {
      var attendance = attendances!.FirstOrDefault(s => s.EntryDate == date);      
      if(attendance == null) {
        // compose attendance based on the shift and timesheet
        // check for timesheet first by date
        TimesheetDTO timesheet = default!;
        using(var scope = serviceScopeFactory.CreateScope())
        {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var ret = await mediator.Send(new GetTimesheetByEmpIdAndDateQuery(request.empId, date));
          if(ret.IsSuccess)
            timesheet = ret.Value;
        }

        // get shift by schedule
        ScheduleDTO schedule = default!;
        using(var scope = serviceScopeFactory.CreateScope())
        {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var ret = await mediator.Send(new GetScheduleByEmpIdAndDateQuery(request.empId, date));
          if(ret.IsSuccess)
            schedule = ret.Value;
        }

        if(schedule == null || schedule.Id == Guid.Empty) // if no schedule found, then try the default schedule.
        {
          using(var scope = serviceScopeFactory.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var ret = await mediator.Send(new GetDefaultScheduleByEmpIdAndDayQuery(request.empId, date.DayOfWeek));
            if(ret.IsSuccess)
              schedule = ret.Value;
          }
        }

        // new attendance to calculate
        attendance = new Attendance(emp.Id, schedule?.ShiftId, timesheet?.Id, null, date, null, null, null, isLocked: false);
        if(schedule != null && schedule.Shift != null)
          attendance.Shift = new Shift("", schedule?.Shift.ShiftStartTime, schedule?.Shift.ShiftBreakTime, schedule?.Shift.ShiftBreakEndTime, schedule?.Shift.ShiftEndTime, schedule?.Shift.BreakHours);
        if(timesheet != null)
          attendance.Timesheet = new Timesheet(Guid.Empty, timesheet.TimeIn1, timesheet.TimeOut1, timesheet.TimeIn2, timesheet.TimeOut2, date);
        
        attendance.CalculateWorkHours();

        results.Add(new AttendanceDTO(Guid.Empty, emp.Id, schedule?.ShiftId, timesheet?.Id, null, date, attendance.WorkHrs, attendance.LateHrs, attendance.UnderHrs, isLocked: false, schedule?.Shift, timesheet));
      }
      else{
        attendance.CalculateWorkHours();

        results.Add(new AttendanceDTO(attendance.Id, emp.Id, attendance.ShiftId, attendance.TimesheetId, attendance.LeaveId, attendance.EntryDate, attendance.WorkHrs, attendance.LateHrs, attendance.UnderHrs, attendance.IsLocked, 
        new ShiftDTO(attendance.Shift.Id, attendance.Shift.Name, attendance.Shift.ShiftStartTime, attendance.Shift.ShiftBreakTime, attendance.Shift.ShiftBreakEndTime, attendance.Shift.ShiftEndTime, attendance.Shift.BreakHours), 
        new TimesheetDTO(attendance.Timesheet.Id, emp.Id, attendance.Timesheet.TimeIn1, attendance.Timesheet.TimeOut1, attendance.Timesheet.TimeIn2, attendance.Timesheet.TimeOut2, attendance.Timesheet.EntryDate, attendance.Timesheet.IsEdited)));
      }
    }
    
    return Result<IEnumerable<AttendanceDTO>>.Success(results.OrderByDescending(i => i.EntryDate));
  }
}
