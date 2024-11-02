using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OvertimeAggregate;

namespace Bluewater.UseCases.Forms.Overtimes.Delete;
public class DeleteOvertimeHandler(IRepository<Overtime> _repository) : ICommandHandler<DeleteOvertimeCommand, Result>
{
  public async Task<Result> Handle(DeleteOvertimeCommand request, CancellationToken cancellationToken)
  {
    Overtime? aggregateToDelete = await _repository.GetByIdAsync(request.OvertimeId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

