using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;

namespace Bluewater.UseCases.Attendances.Update;
public class UpdateAttendanceHandler(IRepository<Attendance> _repository) : ICommandHandler<UpdateAttendanceCommand, Result<AttendanceDTO>>
{
  public async Task<Result<AttendanceDTO>> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
  {
    var spec = new AttendanceByEmpIdAndDateSpec(request.EmployeeId, request.EntryDate);
    var existingAttendance = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (existingAttendance == null) return Result.NotFound();

    existingAttendance.Update(request.ShiftId, request.TimesheetId, request.LeaveId, existingAttendance.WorkHrs, existingAttendance.LateHrs, existingAttendance.UnderHrs, existingAttendance.Remarks);

    await _repository.UpdateAsync(existingAttendance, cancellationToken);

    return Result.Success(new AttendanceDTO(existingAttendance.Id, existingAttendance.EmployeeId, existingAttendance.ShiftId, existingAttendance.TimesheetId, existingAttendance.LeaveId, existingAttendance.EntryDate, 
    existingAttendance.WorkHrs, existingAttendance.LateHrs, existingAttendance.UnderHrs, existingAttendance.IsLocked, 
    new ShiftDTO(existingAttendance.Shift.Id, existingAttendance.Shift.Name, existingAttendance.Shift.ShiftStartTime, existingAttendance.Shift.ShiftBreakTime, existingAttendance.Shift.ShiftBreakEndTime, existingAttendance.Shift.ShiftEndTime, existingAttendance.Shift.BreakHours), 
    new TimesheetDTO(existingAttendance.Timesheet.Id, existingAttendance.EmployeeId, existingAttendance.Timesheet.TimeIn1, existingAttendance.Timesheet.TimeOut1, existingAttendance.Timesheet.TimeIn2, existingAttendance.Timesheet.TimeOut2, existingAttendance.Timesheet.EntryDate, existingAttendance.Timesheet.IsEdited)));
  }
}
