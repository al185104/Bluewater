using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Timesheets.Update;
public class UpdateTimesheetHandler(IRepository<Timesheet> _repository) : ICommandHandler<UpdateTimesheetCommand, Result<TimesheetDTO>>
{
  public async Task<Result<TimesheetDTO>> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
  {
    Timesheet? existingTimesheet = null;

    if (request.TimesheetId != Guid.Empty)
    {
      existingTimesheet = await _repository.GetByIdAsync(request.TimesheetId, cancellationToken);
      if (existingTimesheet == null)
      {
        return Result.NotFound();
      }
    }
    else if (request.entryDate.HasValue)
    {
      var spec = new TimesheetByEmpIdAndEntryDate(request.employeeId, request.entryDate.Value);
      existingTimesheet = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
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

      await _repository.AddAsync(newTimesheet, cancellationToken);

      return Result.Success(new TimesheetDTO(
        newTimesheet.Id,
        newTimesheet.EmployeeId,
        newTimesheet.TimeIn1,
        newTimesheet.TimeOut1,
        newTimesheet.TimeIn2,
        newTimesheet.TimeOut2,
        newTimesheet.EntryDate,
        newTimesheet.IsEdited));
    }

    existingTimesheet.UpdateTimesheet(request.employeeId, request.timeIn1, request.timeOut1, request.timeIn2, request.timeOut2, request.entryDate, request.isLocked);

    await _repository.UpdateAsync(existingTimesheet, cancellationToken);

    return Result.Success(new TimesheetDTO(existingTimesheet.Id, existingTimesheet.EmployeeId, existingTimesheet.TimeIn1, existingTimesheet.TimeOut1, existingTimesheet.TimeIn2, existingTimesheet.TimeOut2, existingTimesheet.EntryDate));
  }
}
