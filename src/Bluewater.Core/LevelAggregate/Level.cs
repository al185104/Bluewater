using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.LevelAggregate;
public class Level(string name, string value) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string Value { get; private set; } = value;
  public bool IsActive { get; private set; } = true;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public Level() : this(string.Empty, string.Empty) { }
}
