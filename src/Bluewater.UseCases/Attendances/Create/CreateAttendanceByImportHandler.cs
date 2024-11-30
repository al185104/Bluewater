using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Attendances.Create;

public class CreateAttendanceByImportHandler(IRepository<Attendance> _attendanceRepo, IRepository<Employee> _employeeRepo, IRepository<Timesheet> _timesheetRepo, IRepository<Shift> _shiftRepo, IRepository<Schedule> _scheduleRepo) : ICommandHandler<CreateAttendanceByImportCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateAttendanceByImportCommand request, CancellationToken cancellationToken)
  {
      var empSpec = new EmployeeByBarcodeSpec(request.attendance.Emp_ID);
      var emp = await _employeeRepo.FirstOrDefaultAsync(empSpec, cancellationToken);
      if(emp == null) return Guid.Empty;

      var shiftSpec = new ShiftByNameSpec(request.attendance.ShiftCode);
      var shift = await _shiftRepo.FirstOrDefaultAsync(shiftSpec, cancellationToken);
      if(shift == null) return Guid.Empty;

      if(string.IsNullOrEmpty(request.attendance.DateSched)) return Guid.Empty;

      string dateFormat = "M/d/yyyy"; 
      var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;

      if (DateOnly.TryParseExact(request.attendance.DateSched, dateFormat, cultureInfo, System.Globalization.DateTimeStyles.None, out DateOnly entryDate)) {

        DateTime? timeIn1 = 
            DateOnly.TryParse(request.attendance.ATimeIn1D, out var dateIn1) &&
            TimeOnly.TryParse(request.attendance.ATimeIn1, out var timeIn1Only)
                ? dateIn1.ToDateTime(timeIn1Only)
                : null;

        DateTime? timeIn2 = 
            DateOnly.TryParse(request.attendance.ATimeIn2D, out var dateIn2) &&
            TimeOnly.TryParse(request.attendance.ATimeIn2, out var timeIn2Only)
                ? dateIn2.ToDateTime(timeIn2Only)
                : null;

        DateTime? timeOut1 = 
            DateOnly.TryParse(request.attendance.ATimeOut1D, out var dateOut1) &&
            TimeOnly.TryParse(request.attendance.ATimeOut1, out var timeOut1Only)
                ? dateOut1.ToDateTime(timeOut1Only)
                : null;

        DateTime? timeOut2 = 
            DateOnly.TryParse(request.attendance.ATimeOut2D, out var dateOut2) &&
            TimeOnly.TryParse(request.attendance.ATimeOut2, out var timeOut2Only)
                ? dateOut2.ToDateTime(timeOut2Only)
                : null;

        // update timesheet
        var timeSpec = new TimesheetByEmpIdAndEntryDate(emp.Id, entryDate);
        var existingTimesheet = await _timesheetRepo.FirstOrDefaultAsync(timeSpec, cancellationToken);
        Guid? timesheetId = null;
        if(existingTimesheet != null) {
            existingTimesheet.UpdateTimesheet(emp.Id, timeIn1, timeOut1, timeIn2, timeOut2, entryDate);
            await _timesheetRepo.UpdateAsync(existingTimesheet, cancellationToken);
            timesheetId = existingTimesheet.Id;
        }
        else {
            var newTimesheet = new Timesheet(emp.Id, timeIn1, timeOut1, timeIn2, timeOut2, entryDate);
            var createdItem = await _timesheetRepo.AddAsync(newTimesheet, cancellationToken);
            timesheetId = createdItem.Id;
        }

        // update schedule
        // check if there's an existing schedule for the date first
        var scheduleSpec = new ScheduleByEmpIdAndDateSpec(emp.Id, entryDate);
        var existingSchedule = await _scheduleRepo.FirstOrDefaultAsync(scheduleSpec, cancellationToken);
        if(existingSchedule != null && existingSchedule.IsDefault == false) {
          //Guid employeeId, Guid shiftId, DateOnly scheduleDate, bool isDefault
          // if yes - update shift
          existingSchedule.UpdateSchedule(emp.Id, shift!.Id, entryDate, isDefault: false);
          await _scheduleRepo.UpdateAsync(existingSchedule, cancellationToken);
        }
        else {
          // if no - check if same as the default.
          var defaultSchedSpec = new DefaultShiftByEmpIdSpec(emp.Id, entryDate.DayOfWeek);
          var defaultSchedule = await _scheduleRepo.FirstOrDefaultAsync(defaultSchedSpec, cancellationToken);
          if(defaultSchedule == null || !defaultSchedule.Shift.Name.Equals(request.attendance.ShiftCode, StringComparison.InvariantCultureIgnoreCase)) {
            // if no default schedule, then just create an override
            var newSchedule = new Schedule(emp.Id, shift!.Id, entryDate, isDefault: false);
            await _scheduleRepo.AddAsync(newSchedule, cancellationToken);
          }
        }

        // update attendance
        var attSpec = new AttendanceByEmpIdAndDateSpec(emp.Id, entryDate);
        var existingAttendance = await _attendanceRepo.FirstOrDefaultAsync(attSpec, cancellationToken);
        if(existingAttendance != null) {
          existingAttendance.Update(shift!.Id, timesheetId, existingAttendance.LeaveId, existingAttendance.WorkHrs, existingAttendance.LateHrs, existingAttendance.UnderHrs, existingAttendance.OverbreakHrs, existingAttendance.NightShiftHours, existingAttendance.Remarks);
          await _attendanceRepo.UpdateAsync(existingAttendance, cancellationToken);
          return existingAttendance.Id;
        }
        else {
          var newAttendance = new Attendance(emp.Id, shift!.Id, timesheetId, null, entryDate, null, null, null, null, null);
          var createdAttendance = await _attendanceRepo.AddAsync(newAttendance, cancellationToken);
          return createdAttendance.Id;
        }
      }

    return Guid.Empty;
  }
}
