using Ardalis.Specification;
using Bluewater.Core.Forms.UndertimeAggregate;

namespace Bluewater.Core.UndertimeAggregate.Specifications;
public class UndertimeAllSpec : Specification<Undertime>
{
  public UndertimeAllSpec()
  {
    Query.Include(Undertime => Undertime.Employee);
  }
}
