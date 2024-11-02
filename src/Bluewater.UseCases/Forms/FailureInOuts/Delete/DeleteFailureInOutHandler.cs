using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.FailureInOutAggregate;

namespace Bluewater.UseCases.Forms.FailureInOuts.Delete;
public class DeleteFailureInOutHandler(IRepository<FailureInOut> _repository) : ICommandHandler<DeleteFailureInOutCommand, Result>
{
  public async Task<Result> Handle(DeleteFailureInOutCommand request, CancellationToken cancellationToken)
  {
    FailureInOut? aggregateToDelete = await _repository.GetByIdAsync(request.FailureInOutId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

