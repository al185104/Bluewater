using Ardalis.Specification;

namespace Bluewater.Core.ShiftAggregate.Specifications;

public sealed class ShiftsByNamesSpec : Specification<Shift>
{
  public ShiftsByNamesSpec(IEnumerable<string> names)
  {
    List<string> values = [.. names
      .Select(name => name.Trim())
      .Where(name => !string.IsNullOrWhiteSpace(name))
      .Distinct(StringComparer.OrdinalIgnoreCase)];

    Query
      .AsNoTracking()
      .Where(shift => values.Contains(shift.Name));
  }
}
