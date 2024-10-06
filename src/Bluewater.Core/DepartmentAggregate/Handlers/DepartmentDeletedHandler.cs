using Bluewater.Core.DepartmentAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.DepartmentAggregate.Handlers;
internal class DepartmentDeletedHandler(ILogger<DepartmentDeletedHandler> _logger, IEmailSender _emailSender) : INotificationHandler<DepartmentDeletedEvent>
{
  public async Task Handle(DepartmentDeletedEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Handling Department Deleted event for {DepartmentId}", notification.DepartmentId);
    await _emailSender.SendEmailAsync("to@test.com",
                                 "from@test.com",
                                 "Department Deleted",
                                 $"Department with id {notification.DepartmentId} was deleted.");
  }
}
