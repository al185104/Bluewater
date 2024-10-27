using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.UseCases.Employees;
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
      if(attendance == null)
        results.Add(new AttendanceDTO(Guid.Empty, emp.Id, null, null, null, date, null, null, null, isLocked: false));
      else
        results.Add(new AttendanceDTO(attendance.Id, emp.Id, attendance.ShiftId, attendance.TimesheetId, attendance.LeaveId, attendance.EntryDate, attendance.WorkHrs, attendance.LateHrs, attendance.UnderHrs, attendance.IsLocked, 
        new Shifts.ShiftDTO(attendance.Shift.Id, attendance.Shift.Name, attendance.Shift.ShiftStartTime, attendance.Shift.ShiftBreakTime, attendance.Shift.ShiftBreakEndTime, attendance.Shift.ShiftEndTime, attendance.Shift.BreakHours), 
        new Timesheets.TimesheetDTO(attendance.Timesheet.Id, emp.Id, attendance.Timesheet.TimeIn1, attendance.Timesheet.TimeOut1, attendance.Timesheet.TimeIn2, attendance.Timesheet.TimeOut2, attendance.Timesheet.EntryDate, attendance.Timesheet.IsEdited)));
    }
    
    return Result<IEnumerable<AttendanceDTO>>.Success(results.OrderByDescending(i => i.EntryDate));
  }
}
