using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;

namespace Bluewater.UseCases.Timesheets.Create;

public class CreateTimesheetHandler(IRepository<Timesheet> _repository) : ICommandHandler<CreateTimesheetCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateTimesheetCommand request, CancellationToken cancellationToken)
  {
    // check first if there's entry already for the same entry date
    var spec = new TimesheetByEmpIdAndEntryDate(request.employeeId, request.entryDate ?? DateOnly.FromDateTime(DateTime.Now));
    var existingItem = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if(existingItem != null)
    {
        // update the existing timesheet
        // create only updates the first time entry.
        existingItem.UpdateTimesheet(request.employeeId, request.timeIn1, existingItem.TimeOut1 ?? request.timeOut1, existingItem.TimeIn2 ?? request.timeIn2, existingItem.TimeOut2 ?? request.timeOut2, request.entryDate);
        await _repository.UpdateAsync(existingItem, cancellationToken);
        return existingItem.Id;
    }
    else {
        // create a new one
        var newTimesheet = new Timesheet(request.employeeId, request.timeIn1, request.timeOut1, request.timeIn2, request.timeOut2, request.entryDate);
        var createdItem = await _repository.AddAsync(newTimesheet, cancellationToken);
        return createdItem.Id;
    }
  }
}
