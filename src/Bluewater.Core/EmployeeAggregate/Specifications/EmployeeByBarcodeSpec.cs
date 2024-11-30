using Ardalis.Specification;

namespace Bluewater.Core.EmployeeAggregate.Specifications;
public class EmployeeByBarcodeSpec : Specification<Employee>
{
  public EmployeeByBarcodeSpec(string barcode)
  {
    Query
        .Include(Employee => Employee.User)
        .Where(i => i.User != null && i.User.Username.ToLower() == barcode.ToLower());
  }
}
