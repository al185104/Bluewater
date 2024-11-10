using Ardalis.Specification;

namespace Bluewater.Core.ServiceChargeAggregate.Specifications;
public class ServiceChargeByDateSpec : Specification<ServiceCharge>
{
  public ServiceChargeByDateSpec(DateOnly endDate)
  {
    Query
        .Where(ServiceCharge => ServiceCharge.Date == endDate);
  }
}
