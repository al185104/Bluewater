using Ardalis.SharedKernel;

namespace Bluewater.Core.ChargingAggregate.Events;
internal sealed class ChargingDeletedEvent(Guid ChargingId) : DomainEventBase
{
  public Guid ChargingId { get; init; } = ChargingId;
}
