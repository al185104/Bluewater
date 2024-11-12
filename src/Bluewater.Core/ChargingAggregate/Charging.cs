using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;

namespace Bluewater.Core.ChargingAggregate;
public class Charging(string name, string? description, Guid? departmentId) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;

  // Navigation Properties
  public Guid? DepartmentId { get; set; } = departmentId;
  public virtual Department? Department { get; set; }

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  public Charging() : this(string.Empty, null, null) { }

  public void UpdateCharging(string name, string? description, Guid? deptId)
  {
    Name = name;
    Description = description;
    DepartmentId = deptId;

    UpdatedDate = DateTime.Now;
  }
}
