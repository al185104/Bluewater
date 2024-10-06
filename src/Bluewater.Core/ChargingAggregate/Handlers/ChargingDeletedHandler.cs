using Bluewater.Core.ChargingAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.ChargingAggregate.Handlers;
internal class ChargingDeletedHandler(ILogger<ChargingDeletedHandler> _logger, IEmailSender _emailSender) : INotificationHandler<ChargingDeletedEvent>
{
  public async Task Handle(ChargingDeletedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling Charging Deleted event for {ChargingId}", notification.ChargingId);
    await _emailSender.SendEmailAsync("to@test.com",
                                 "from@test.com",
                                 "Charging Deleted",
                                 $"Charging with id {notification.ChargingId} was deleted.");
    await Task.Delay(100);
  }
}
