using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.MealCredits;

public class DeleteMealCreditRequest
{
  public const string Route = "/MealCredits/{MealCreditId:guid}";

  [Required]
  public Guid MealCreditId { get; set; }
}
