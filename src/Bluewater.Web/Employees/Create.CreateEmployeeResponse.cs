using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.Web.Employees;

public class CreateEmployeeResponse(Guid Id, string FirstName, string LastName, string? MiddleName, Tenant Tenant)
{
  public Guid Id { get; set; } = Id;
  public string FirstName { get; set; } = FirstName;
  public string LastName { get; set; } = LastName;
  public string? MiddleName { get; set; } = MiddleName;
  public Tenant Tenant { get; set; } = Tenant;
}
