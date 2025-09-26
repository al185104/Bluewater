using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Pays;

public class CreatePayRequest
{
  public const string Route = "/Pays";

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
}
