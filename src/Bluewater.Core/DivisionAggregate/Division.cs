using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;

namespace Bluewater.Core.DivisionAggregate;

public class Division(string name, string? description) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public virtual ICollection<Department>? Departments { get; private set; }

  public Division() : this(string.Empty, null) { }

  public void UpdateDivision(string newName, string? newDescription)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    Description = newDescription;
    UpdatedDate = DateTime.Now;
  }
}
