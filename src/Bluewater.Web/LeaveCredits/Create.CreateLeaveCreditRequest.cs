using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.LeaveCredits;

public class CreateLeaveCreditRequest
{
  public const string Route = "/LeaveCredits";

  [Required]
  public string? Code { get; set; }

  [Required]
  public string? Description { get; set; }

  [Range(0, double.MaxValue)]
  public decimal? Credit { get; set; }

  public int? SortOrder { get; set; }

  public bool IsLeaveWithPay { get; set; }

  public bool IsCanCarryOver { get; set; }
}
