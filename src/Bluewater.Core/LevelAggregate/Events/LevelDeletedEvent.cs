using Ardalis.SharedKernel;

namespace Bluewater.Core.LevelAggregate.Events;
/// <summary>
/// A domain event that is dispatched whenever a Level is deleted.
/// The DeleteLevelService is used to dispatch this event.
/// </summary>
internal sealed class LevelDeletedEvent(Guid LevelId) : DomainEventBase
{
  public Guid LevelId { get; init; } = LevelId;
}
