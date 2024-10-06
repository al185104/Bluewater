using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.SectionAggregate;

namespace Bluewater.Core.DepartmentAggregate;
public class Department(string name, string? description, Guid divisionId) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public Guid DivisionId { get; set; } = divisionId;
  public virtual Division? Division { get; set; }

  public virtual ICollection<Section>? Sections { get; private set; }

  public Department() : this(string.Empty, null, Guid.Empty) { }

  public void UpdateDepartment(string newName, string? newDescription, Guid divisionId)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    Description = newDescription;
    DivisionId = Guard.Against.Default(divisionId, nameof(divisionId));
    UpdatedDate = DateTime.Now;
  }
}
