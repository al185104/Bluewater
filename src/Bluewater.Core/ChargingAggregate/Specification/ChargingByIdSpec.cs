using Ardalis.Specification;

namespace Bluewater.Core.ChargingAggregate.Specifications;
public class ChargingByIdSpec : Specification<Charging>
{
  public ChargingByIdSpec(Guid ChargingId)
  {
    Query
        .Where(Charging => Charging.Id == ChargingId);
  }
}
