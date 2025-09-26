using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.MealCreditAggregate;

namespace Bluewater.UseCases.MealCredits.Update;

public class UpdateMealCreditHandler(IRepository<MealCredit> _repository) : ICommandHandler<UpdateMealCreditCommand, Result<MealCreditDTO>>
{
  public async Task<Result<MealCreditDTO>> Handle(UpdateMealCreditCommand request, CancellationToken cancellationToken)
  {
    MealCredit? existing = await _repository.GetByIdAsync(request.MealCreditId, cancellationToken);
    if (existing is null)
    {
      return Result.NotFound();
    }

    existing.UpdateMealCredit(request.EmployeeId, request.Date, request.Count);
    await _repository.UpdateAsync(existing, cancellationToken);

    return Result.Success(new MealCreditDTO(existing.Id, existing.EmployeeId, existing.Date, existing.Count));
  }
}
