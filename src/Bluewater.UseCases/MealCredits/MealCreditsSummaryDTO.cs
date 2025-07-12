namespace Bluewater.UseCases.MealCredits;
public record MealCreditsSummaryDTO
{
    public string? EmployeeName { get; init; }
    public int? MealCount { get; init; }
}
