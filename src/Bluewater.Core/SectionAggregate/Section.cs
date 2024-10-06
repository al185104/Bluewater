using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Bluewater.Core.DepartmentAggregate;
using Bluewater.Core.PositionAggregate;

namespace Bluewater.Core.SectionAggregate;
public class Section(string name, string? description, string? approved1id, string? approved2id, string? approved3id, Guid departmentId) : EntityBase<Guid>, IAggregateRoot
{
  public string Name { get; private set; } = name;
  public string? Description { get; private set; } = description;
  public string? Approved1Id { get; private set; } = approved1id;
  public string? Approved2Id { get; private set; } = approved2id;
  public string? Approved3Id { get; private set; } = approved3id;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Properties
  public Guid DepartmentId { get; set; } = departmentId;
  public virtual Department? Department { get; set; }

  public virtual ICollection<Position>? Positions { get; set; }

  public Section() : this(string.Empty, null, null, null, null, Guid.Empty) {}

  public void UpdateSection(string newName, string? newDescription, string? newApproved1Id, string? newApproved2Id, string? newApproved3Id, Guid departmentId)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    Description = newDescription;
    Approved1Id = newApproved1Id;
    Approved2Id = newApproved2Id;
    Approved3Id = newApproved3Id;
    DepartmentId = Guard.Against.Default(departmentId, nameof(departmentId));
    UpdatedDate = DateTime.Now;
  }
}
