using Ardalis.Result;

namespace Bluewater.UseCases.MealCredits.Create;

public record CreateMealCreditCommand(Guid? EmployeeId, DateOnly? Date, int? Count) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
