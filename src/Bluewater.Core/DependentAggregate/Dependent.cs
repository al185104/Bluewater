using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.DependentAggregate;
public class Dependent(string firstName, string lastName, string relationship, DateTime? dateOfBirth) : EntityBase<Guid>, IAggregateRoot
{
  public string FirstName { get; private set; } = firstName;
  public string LastName { get; private set; } = lastName;
  public string Relationship { get; private set; } = relationship;
  public DateTime? DateOfBirth { get; private set; } = dateOfBirth;

  // Navigation Property
  public Guid EmployeeId { get; set; }
  public virtual Employee Employee { get; set; } = null!;

  public Dependent() : this(string.Empty, string.Empty, string.Empty, null) { }
}
