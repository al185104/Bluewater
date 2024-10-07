using Ardalis.SharedKernel;

namespace Bluewater.Core.EmployeeTypeAggregate.Events;
/// <summary>
/// A domain event that is dispatched whenever a EmployeeType is deleted.
/// The DeleteEmployeeTypeService is used to dispatch this event.
/// </summary>
internal sealed class EmployeeTypeDeletedEvent(Guid EmployeeTypeId) : DomainEventBase
{
  public Guid EmployeeTypeId { get; init; } = EmployeeTypeId;
}
