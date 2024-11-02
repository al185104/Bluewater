using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.OtherEarningAggregate;

namespace Bluewater.UseCases.Forms.OtherEarnings.Delete;
public class DeleteOtherEarningHandler(IRepository<OtherEarning> _repository) : ICommandHandler<DeleteOtherEarningCommand, Result>
{
  public async Task<Result> Handle(DeleteOtherEarningCommand request, CancellationToken cancellationToken)
  {
    OtherEarning? aggregateToDelete = await _repository.GetByIdAsync(request.OtherEarningId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

