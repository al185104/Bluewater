using Bluewater.UseCases.ServiceCharges;

namespace Bluewater.Web.ServiceCharges;

public static class ServiceChargeMapper
{
  public static ServiceChargeRecord ToRecord(ServiceChargeDTO dto)
  {
    return new ServiceChargeRecord(dto.Id, dto.Username, dto.Amount, dto.Date);
  }
}
