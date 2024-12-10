using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.MealCredits;
public class CreateMealCreditRequest
{
  public const string Route = "/MealCredits";

  [Required]
  public string barcode { get; set; } = string.Empty;
  public DateOnly? entryDate { get; set; }
  public int count { get; set; }
}
