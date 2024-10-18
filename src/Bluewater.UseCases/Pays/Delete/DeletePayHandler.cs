using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayAggregate;

namespace Bluewater.UseCases.Pays.Delete;
public class DeletePayHandler(IRepository<Pay> _repository) : ICommandHandler<DeletePayCommand, Result>
{
  public async Task<Result> Handle(DeletePayCommand request, CancellationToken cancellationToken)
  {
    Pay? aggregateToDelete = await _repository.GetByIdAsync(request.PayId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

