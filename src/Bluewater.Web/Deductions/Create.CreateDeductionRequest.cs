using System.ComponentModel.DataAnnotations;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.Web.Deductions;

public class CreateDeductionRequest
{
  public const string Route = "/Deductions";

  [Required]
  public Guid EmpId { get; set; }

  public DeductionsTypeDTO? Type { get; set; } = DeductionsTypeDTO.NotSet;

  public decimal? TotalAmount { get; set; }

  public decimal? MonthlyAmortization { get; set; }

  public decimal? RemainingBalance { get; set; }

  public int? NoOfMonths { get; set; }

  public DateTime? StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  public string? Remarks { get; set; }
}
