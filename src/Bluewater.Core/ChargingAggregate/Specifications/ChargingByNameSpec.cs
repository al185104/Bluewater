using Ardalis.Specification;

namespace Bluewater.Core.ChargingAggregate.Specifications;

public class ChargingByNameSpec : Specification<Charging>
{
  public ChargingByNameSpec(string name)
  {
    var normalizedName = name.Trim();

    Query.Where(charging => charging.Name.ToLower() == normalizedName.ToLower());
  }
}
