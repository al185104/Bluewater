using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Pays;

public class UpdatePayRequest
{
  public const string Route = "/Pays/{PayId:guid}";
  public static string BuildRoute(Guid payId) => Route.Replace("{PayId:guid}", payId.ToString());

  [Required]
  public Guid PayId { get; set; }

  [Range(0, double.MaxValue)]
  public decimal BasicPay { get; set; }
  [Range(0, double.MaxValue)]
  public decimal DailyRate { get; set; }
  [Range(0, double.MaxValue)]
  public decimal HourlyRate { get; set; }
  [Range(0, double.MaxValue)]
  public decimal HdmfCon { get; set; }
  [Range(0, double.MaxValue)]
  public decimal HdmfEr { get; set; }
  [Range(0, double.MaxValue)]
  public decimal Cola { get; set; }
}
