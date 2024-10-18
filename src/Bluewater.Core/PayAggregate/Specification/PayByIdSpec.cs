using Ardalis.Specification;

namespace Bluewater.Core.PayAggregate.Specifications;
public class PayByIdSpec : Specification<Pay>
{
  public PayByIdSpec(Guid PayId)
  {
    Query
        .Where(Pay => Pay.Id == PayId);
  }
}
