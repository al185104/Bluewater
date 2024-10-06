using Ardalis.SharedKernel;

namespace Bluewater.Core.SectionAggregate.Events;
internal sealed class SectionDeletedEvent(Guid SectionId) : DomainEventBase
{
  public Guid SectionId { get; init; } = SectionId;
}
