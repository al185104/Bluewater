using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Timesheets.Update;
public class UpdateSingleTimesheetHandler(IRepository<Timesheet> _repository) : ICommandHandler<UpdateSingleTimesheetCommand, Result<TimesheetDTO>>
{
  public async Task<Result<TimesheetDTO>> Handle(UpdateSingleTimesheetCommand request, CancellationToken cancellationToken)
  {
    var spec = new TimesheetByEmpIdAndEntryDate(request.EmployeeId, request.entryDate);
    var existingTimesheet = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (existingTimesheet == null) return Result.NotFound();

    existingTimesheet.UpdateTimesheetByInput(request.EmployeeId, request.time, (int)request.input);

    await _repository.UpdateAsync(existingTimesheet, cancellationToken);

    return Result.Success(new TimesheetDTO(existingTimesheet.Id, existingTimesheet.EmployeeId, existingTimesheet.TimeIn1, existingTimesheet.TimeOut1, existingTimesheet.TimeIn2, existingTimesheet.TimeOut2, existingTimesheet.EntryDate, true));
  }
}
