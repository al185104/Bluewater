namespace Bluewater.Web.Pays;

public class CreatePayResponse(PayRecord Pay)
{
  public PayRecord Pay { get; set; } = Pay;
}
