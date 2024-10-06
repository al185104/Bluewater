using System.Reflection.Metadata.Ecma335;
using Bluewater.Core.DivisionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.DivisionAggregate.Handlers;
internal class DivisionDeletedHandler(ILogger<DivisionDeletedHandler> _logger, IEmailSender _emailSender) : INotificationHandler<DivisionDeletedEvent>
{
  public async Task Handle(DivisionDeletedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling Division Deleted event for {divisionId}", notification.DivisionId);
    await _emailSender.SendEmailAsync("to@test.com",
                                 "from@test.com",
                                 "Division Deleted",
                                 $"Division with id {notification.DivisionId} was deleted.");
  }
}
