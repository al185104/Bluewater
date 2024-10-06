using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ShiftAggregate;

namespace Bluewater.UseCases.Shifts.Delete;
public class DeleteShiftHandler(IRepository<Shift> _repository) : ICommandHandler<DeleteShiftCommand, Result>
{
  public async Task<Result> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
  {
    Shift? aggregateToDelete = await _repository.GetByIdAsync(request.ShiftId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

