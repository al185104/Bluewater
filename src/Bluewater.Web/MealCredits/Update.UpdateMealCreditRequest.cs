using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.MealCredits;

public class UpdateMealCreditRequest
{
  public const string Route = "/MealCredits";

  [Required]
  public Guid Id { get; set; }

  public Guid? EmployeeId { get; set; }

  public DateOnly? Date { get; set; }

  public int? Count { get; set; }
}
