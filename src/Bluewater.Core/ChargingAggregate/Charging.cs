using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.ChargingAggregate;
public class Charging(string name, string? description) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public Charging() : this(string.Empty, null) { }

  public void UpdateCharging(string name, string? description)
  {
    Name = name;
    Description = description;
    UpdatedDate = DateTime.Now;
  }
}
