using Bluewater.Core.PositionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.PositionAggregate.Handlers;
internal class PositionDeletedHandler(ILogger<PositionDeletedHandler> _logger, IEmailSender _emailSender) : INotificationHandler<PositionDeletedEvent>
{
  public async Task Handle(PositionDeletedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling Position Deleted event for {PositionId}", notification.PositionId);
    await _emailSender.SendEmailAsync("to@test.com",
                                 "from@test.com",
                                 "Position Deleted",
                                 $"Position with id {notification.PositionId} was deleted.");
  }
}
