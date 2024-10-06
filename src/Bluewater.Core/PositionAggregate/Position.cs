using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Bluewater.Core.DivisionAggregate;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.SectionAggregate;

namespace Bluewater.Core.PositionAggregate;
public class Position(string name, string? description, Guid sectionId) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public Guid SectionId { get; private set; } = sectionId;
  public virtual Section? Section { get; private set; }

  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public Position() : this(string.Empty, null, Guid.Empty) { }

  public void UpdatePosition(string name, string? description, Guid sectionId)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    Description = description;
    SectionId = Guard.Against.Default(sectionId, nameof(sectionId));
    UpdatedDate = DateTime.Now;
  }
}
