using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Timesheets.Update;
public class UpdateTimesheetHandler(
  IRepository<Timesheet> timesheetRepository,
  IRepository<Schedule> scheduleRepository,
  IRepository<Shift> shiftRepository) : ICommandHandler<UpdateTimesheetCommand, Result<TimesheetDTO>>
{
  public async Task<Result<TimesheetDTO>> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
  {
    Timesheet? existingTimesheet = null;

    if (request.TimesheetId != Guid.Empty)
    {
      existingTimesheet = await timesheetRepository.GetByIdAsync(request.TimesheetId, cancellationToken);
      if (existingTimesheet == null)
      {
        return Result.NotFound();
      }
    }
    else if (request.entryDate.HasValue)
    {
      var spec = new TimesheetByEmpIdAndEntryDate(request.employeeId, request.entryDate.Value);
      existingTimesheet = await timesheetRepository.FirstOrDefaultAsync(spec, cancellationToken);
    }

    if (existingTimesheet is null)
    {
      var newTimesheet = new Timesheet(
        request.employeeId,
        request.timeIn1,
        request.timeOut1,
        request.timeIn2,
        request.timeOut2,
        request.entryDate);

      await timesheetRepository.AddAsync(newTimesheet, cancellationToken);
      existingTimesheet = newTimesheet;
    }
    else
    {
      existingTimesheet.UpdateTimesheet(request.employeeId, request.timeIn1, request.timeOut1, request.timeIn2, request.timeOut2, request.entryDate, request.isLocked);
      await timesheetRepository.UpdateAsync(existingTimesheet, cancellationToken);
    }

    (Guid? scheduleId, Guid? shiftId, string? shiftName) = await UpsertScheduleAsync(request, cancellationToken);

    return Result.Success(new TimesheetDTO(
      existingTimesheet.Id,
      existingTimesheet.EmployeeId,
      existingTimesheet.TimeIn1,
      existingTimesheet.TimeOut1,
      existingTimesheet.TimeIn2,
      existingTimesheet.TimeOut2,
      existingTimesheet.EntryDate,
      existingTimesheet.IsEdited,
      scheduleId,
      shiftId,
      shiftName));

    async Task<(Guid? scheduleId, Guid? shiftId, string? shiftName)> UpsertScheduleAsync(UpdateTimesheetCommand command, CancellationToken token)
    {
      if (!command.entryDate.HasValue)
      {
        return (null, null, null);
      }

      var scheduleSpec = new ScheduleByEmpIdAndDateSpec(command.employeeId, command.entryDate.Value);
      Schedule? existingSchedule = await scheduleRepository.FirstOrDefaultAsync(scheduleSpec, token);

      if (!command.shiftId.HasValue || command.shiftId == Guid.Empty)
      {
        return (existingSchedule?.Id, existingSchedule?.ShiftId, existingSchedule?.Shift?.Name);
      }

      Shift? selectedShift = await shiftRepository.GetByIdAsync(command.shiftId.Value, token);
      if (selectedShift is null)
      {
        return (existingSchedule?.Id, existingSchedule?.ShiftId, existingSchedule?.Shift?.Name);
      }

      if (existingSchedule is null)
      {
        var createdSchedule = new Schedule(command.employeeId, selectedShift.Id, command.entryDate.Value, isDefault: false);
        await scheduleRepository.AddAsync(createdSchedule, token);
        return (createdSchedule.Id, selectedShift.Id, selectedShift.Name);
      }

      existingSchedule.UpdateSchedule(command.employeeId, selectedShift.Id, command.entryDate.Value, existingSchedule.IsDefault);
      await scheduleRepository.UpdateAsync(existingSchedule, token);
      return (existingSchedule.Id, selectedShift.Id, selectedShift.Name);
    }
  }
}
