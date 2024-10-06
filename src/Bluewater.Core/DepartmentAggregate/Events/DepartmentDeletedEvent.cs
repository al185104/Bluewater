using Ardalis.SharedKernel;

namespace Bluewater.Core.DepartmentAggregate.Events;
internal sealed class DepartmentDeletedEvent(Guid DepartmentId) : DomainEventBase
{
  public Guid DepartmentId { get; init; } = DepartmentId;
}
