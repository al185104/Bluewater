using Ardalis.SharedKernel;

namespace Bluewater.Core.MealCreditAggregate;
public class MealCredit(Guid? EmployeeId, DateOnly? Date, int? Count) : EntityBase<Guid>, IAggregateRoot
{
  public Guid? EmployeeId {get; private set; } = EmployeeId;
  public DateOnly? Date { get; private set; } = Date;
  public int? Count { get; private set; } = Count;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  public MealCredit() : this(null, null, null) { }

  public void UpdateMealCredit(Guid? employeeId, DateOnly? date, int? count)
  {
    EmployeeId = employeeId;
    Date = date;
    Count = count;

    UpdatedDate = DateTime.Now;
  }
}
