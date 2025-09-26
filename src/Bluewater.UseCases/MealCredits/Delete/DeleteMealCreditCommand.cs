using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.MealCredits.Delete;

public record DeleteMealCreditCommand(Guid MealCreditId) : ICommand<Result>;
