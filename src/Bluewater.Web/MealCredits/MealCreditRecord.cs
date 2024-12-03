namespace Bluewater.Web.MealCredits;

public record MealCreditRecord(Guid Id, Guid? EmployeeId, DateOnly? Date, int? Count);
