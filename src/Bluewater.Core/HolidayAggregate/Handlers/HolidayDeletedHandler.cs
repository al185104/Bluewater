using Bluewater.Core.HolidayAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.HolidayAggregate.Handlers;
/// <summary>
/// NOTE: Internal because HolidayDeleted is also marked as internal.
/// </summary>
internal class HolidayDeletedHandler(ILogger<HolidayDeletedHandler> logger, IEmailSender emailSender) : INotificationHandler<HolidayDeletedEvent>
{
  public async Task Handle(HolidayDeletedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Contributed Deleted event for {HolidayId}", domainEvent.HolidayId);

    await emailSender.SendEmailAsync("to@test.com",
                                     "from@test.com",
                                     "Holiday Deleted",
                                     $"Holiday with id {domainEvent.HolidayId} was deleted.");
  }
}
