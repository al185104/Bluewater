using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.UseCases.Forms.Undertimes.Delete;
public class DeleteUndertimeHandler(IRepository<Undertime> _repository) : ICommandHandler<DeleteUndertimeCommand, Result>
{
  public async Task<Result> Handle(DeleteUndertimeCommand request, CancellationToken cancellationToken)
  {
    Undertime? aggregateToDelete = await _repository.GetByIdAsync(request.UndertimeId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

