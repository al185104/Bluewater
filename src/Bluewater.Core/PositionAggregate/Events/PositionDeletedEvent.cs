using Ardalis.SharedKernel;

namespace Bluewater.Core.PositionAggregate.Events;
internal sealed class PositionDeletedEvent(Guid PositionId) : DomainEventBase
{
  public Guid PositionId { get; init; } = PositionId;
}
