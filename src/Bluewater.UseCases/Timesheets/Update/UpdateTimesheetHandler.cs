using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;

namespace Bluewater.UseCases.Timesheets.Update;
public class UpdateTimesheetHandler(IRepository<Timesheet> _repository) : ICommandHandler<UpdateTimesheetCommand, Result<TimesheetDTO>>
{
  public async Task<Result<TimesheetDTO>> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
  {
    var existingTimesheet = await _repository.GetByIdAsync(request.TimesheetId, cancellationToken);
    if (existingTimesheet == null)
      return Result.NotFound();

    existingTimesheet.UpdateTimesheet(request.employeeId, request.timeIn1, request.timeOut1, request.timeIn2, request.timeOut2, request.entryDate);

    await _repository.UpdateAsync(existingTimesheet, cancellationToken);

    return Result.Success(new TimesheetDTO(existingTimesheet.Id, existingTimesheet.EmployeeId, existingTimesheet.TimeIn1, existingTimesheet.TimeOut1, existingTimesheet.TimeIn2, existingTimesheet.TimeOut2, existingTimesheet.EntryDate));
  }
}
