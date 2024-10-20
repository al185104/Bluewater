using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ScheduleAggregate;

namespace Bluewater.UseCases.Schedules.Delete;
public class DeleteScheduleHandler(IRepository<Schedule> _repository) : ICommandHandler<DeleteScheduleCommand, Result>
{
  public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
  {
    Schedule? aggregateToDelete = await _repository.GetByIdAsync(request.ScheduleId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

