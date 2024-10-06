using Bluewater.Core.SectionAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.SectionAggregate.Handlers;
internal class SectionDeletedHandler(ILogger<SectionDeletedHandler> _logger, IEmailSender _emailSender) : INotificationHandler<SectionDeletedEvent>
{
  public async Task Handle(SectionDeletedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling Section Deleted event for {SectionId}", notification.SectionId);
    await _emailSender.SendEmailAsync("to@test.com",
                                 "from@test.com",
                                 "Section Deleted",
                                 $"Section with id {notification.SectionId} was deleted.");
  }
}
