using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.EmployeeTypeAggregate;
public class EmployeeType(string name, string value, bool isActive) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string Value { get; private set; } = value;
  public bool IsActive { get; private set; } = isActive;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public EmployeeType() : this(string.Empty, string.Empty, false) { }

  public void UpdateEmployeeType(string name, string value, bool isActive)
  {
    Name = name;
    Value = value;
    IsActive = isActive;
  }
}
