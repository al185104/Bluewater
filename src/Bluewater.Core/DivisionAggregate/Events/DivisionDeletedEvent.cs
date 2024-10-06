using Ardalis.SharedKernel;

namespace Bluewater.Core.DivisionAggregate.Events;
internal sealed class DivisionDeletedEvent(Guid divisionId) : DomainEventBase
{
  public Guid DivisionId { get; init; } = divisionId;
}
