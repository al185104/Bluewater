using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.MealCredits.Get;

public record GetMealCreditQuery(Guid MealCreditId) : IQuery<Result<MealCreditDTO>>;
