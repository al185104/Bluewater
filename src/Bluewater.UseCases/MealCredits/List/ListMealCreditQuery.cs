using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.MealCredits.List;
public record ListMealCreditQuery(int? skip, int? take) : IQuery<Result<IEnumerable<MealCreditDTO>>>;
