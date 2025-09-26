using Bluewater.UseCases.Pays;

namespace Bluewater.Web.Pays;

public static class PayMapper
{
  public static PayRecord ToRecord(PayDTO dto)
  {
    return new PayRecord(
      dto.Id,
      dto.BasicPay,
      dto.DailyRate,
      dto.HourlyRate,
      dto.HDMF_Con,
      dto.HDMF_Er,
      dto.Cola);
  }
}
