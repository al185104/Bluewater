using Ardalis.SharedKernel;

namespace Bluewater.Core.HolidayAggregate.Events;
/// <summary>
/// A domain event that is dispatched whenever a Holiday is deleted.
/// The DeleteHolidayService is used to dispatch this event.
/// </summary>
internal sealed class HolidayDeletedEvent(Guid HolidayId) : DomainEventBase
{
  public Guid HolidayId { get; init; } = HolidayId;
}
