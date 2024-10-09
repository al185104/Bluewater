using Ardalis.SharedKernel;

namespace Bluewater.Core.EmployeeAggregate.Events;
/// <summary>
/// A domain event that is dispatched whenever a Employee is deleted.
/// The DeleteEmployeeService is used to dispatch this event.
/// </summary>
internal sealed class EmployeeDeletedEvent(Guid EmployeeId) : DomainEventBase
{
  public Guid EmployeeId { get; init; } = EmployeeId;
}
