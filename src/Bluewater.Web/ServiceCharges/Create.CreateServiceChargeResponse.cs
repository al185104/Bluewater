namespace Bluewater.Web.ServiceCharges;

public class CreateServiceChargeResponse(ServiceChargeRecord ServiceCharge)
{
  public ServiceChargeRecord ServiceCharge { get; set; } = ServiceCharge;
}
