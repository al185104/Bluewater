namespace Bluewater.Web.Pays;

public class UpdatePayResponse(PayRecord Pay)
{
  public PayRecord Pay { get; set; } = Pay;
}
