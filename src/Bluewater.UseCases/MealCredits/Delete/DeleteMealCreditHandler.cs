using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.MealCreditAggregate;

namespace Bluewater.UseCases.MealCredits.Delete;

public class DeleteMealCreditHandler(IRepository<MealCredit> _repository) : ICommandHandler<DeleteMealCreditCommand, Result>
{
  public async Task<Result> Handle(DeleteMealCreditCommand request, CancellationToken cancellationToken)
  {
    MealCredit? aggregateToDelete = await _repository.GetByIdAsync(request.MealCreditId, cancellationToken);
    if (aggregateToDelete is null)
    {
      return Result.NotFound();
    }

    await _repository.DeleteAsync(aggregateToDelete, cancellationToken);
    return Result.Success();
  }
}
