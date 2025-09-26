using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.MealCredits.Update;

public record UpdateMealCreditCommand(Guid MealCreditId, Guid? EmployeeId, DateOnly? Date, int? Count) : ICommand<Result<MealCreditDTO>>;
