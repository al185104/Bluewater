using Ardalis.Result;

namespace Bluewater.UseCases.MealCredits.Create;

public record CreateMealCreditCommand(string barcode, DateOnly? Date, int? Count) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
