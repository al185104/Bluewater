using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.DeductionAggregate;

namespace Bluewater.UseCases.Forms.Deductions.Delete;
public class DeleteDeductionHandler(IRepository<Deduction> _repository) : ICommandHandler<DeleteDeductionCommand, Result>
{
  public async Task<Result> Handle(DeleteDeductionCommand request, CancellationToken cancellationToken)
  {
    Deduction? aggregateToDelete = await _repository.GetByIdAsync(request.DeductionId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

