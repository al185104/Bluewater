using Ardalis.Specification;
using Bluewater.Core.EmployeeAggregate.Enum;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate.Specifications;

public sealed class EmployeesByBarcodesAndTenantSpec : Specification<Employee>
{
  public EmployeesByBarcodesAndTenantSpec(IEnumerable<string> barcodes, Tenant tenant)
  {
    List<string> values = [.. barcodes
      .Select(barcode => barcode.Trim())
      .Where(barcode => !string.IsNullOrWhiteSpace(barcode))
      .Distinct(StringComparer.OrdinalIgnoreCase)];

    Query
      .AsNoTracking()
      .Include(employee => employee.User)
      .Where(employee =>
        employee.Tenant == tenant &&
        !employee.IsDeleted &&
        employee.User != null &&
        values.Contains(employee.User.Username));
  }
}
