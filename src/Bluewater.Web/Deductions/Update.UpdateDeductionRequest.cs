using System.ComponentModel.DataAnnotations;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.Web.Deductions;

public class UpdateDeductionRequest
{
  public const string Route = "/Deductions/{DeductionId:guid}";

  public static string BuildRoute(Guid deductionId) => Route.Replace("{DeductionId:guid}", deductionId.ToString());

  public Guid DeductionId { get; set; }

  [Required]
  public Guid Id { get; set; }

  [Required]
  public Guid EmpId { get; set; }

  [Required]
  public DeductionsTypeDTO Type { get; set; } = DeductionsTypeDTO.NotSet;

  [Required]
  public decimal TotalAmount { get; set; }

  [Required]
  public decimal MonthlyAmortization { get; set; }

  [Required]
  public decimal RemainingBalance { get; set; }

  [Required]
  public int NoOfMonths { get; set; }

  [Required]
  public DateTime StartDate { get; set; }

  [Required]
  public DateTime EndDate { get; set; }

  [Required]
  public string Remarks { get; set; } = string.Empty;

  [Required]
  public ApplicationStatusDTO Status { get; set; } = ApplicationStatusDTO.NotSet;
}
