using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.MealCreditAggregate;

namespace Bluewater.UseCases.MealCredits.Get;

public class GetMealCreditHandler(IRepository<MealCredit> _repository) : IQueryHandler<GetMealCreditQuery, Result<MealCreditDTO>>
{
  public async Task<Result<MealCreditDTO>> Handle(GetMealCreditQuery request, CancellationToken cancellationToken)
  {
    MealCredit? mealCredit = await _repository.GetByIdAsync(request.MealCreditId, cancellationToken);
    if (mealCredit is null)
    {
      return Result.NotFound();
    }

    return Result.Success(new MealCreditDTO(mealCredit.Id, mealCredit.EmployeeId, mealCredit.Date, mealCredit.Count));
  }
}
