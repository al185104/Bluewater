namespace Bluewater.UseCases.MealCredits;
public record MealCreditDTO()
{
  public Guid Id { get; init; }
  public Guid? EmployeeId { get; set; }
  public DateOnly? Date { get; set; }
  public int? Count { get; set; }

  public MealCreditDTO(Guid id, Guid? empId, DateOnly? date, int? count) : this()
  {
    Id = id;
    EmployeeId = empId;
    Date = date;
    Count = count;
  }
}
