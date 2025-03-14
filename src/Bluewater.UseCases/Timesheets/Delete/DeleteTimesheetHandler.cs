using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UseCases.Timesheets.Delete;

namespace Bluewater.UseCases.Shifts.Delete;
public class DeleteTimesheetHandler(IRepository<Timesheet> _repository) : ICommandHandler<DeleteTimesheetCommand, Result>
{
  public async Task<Result> Handle(DeleteTimesheetCommand request, CancellationToken cancellationToken)
  {
    Timesheet? aggregateToDelete = await _repository.GetByIdAsync(request.TimesheetId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

