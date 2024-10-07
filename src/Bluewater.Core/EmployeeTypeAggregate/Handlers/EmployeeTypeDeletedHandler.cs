using Bluewater.Core.EmployeeTypeAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.EmployeeTypeAggregate.Handlers;
/// <summary>
/// NOTE: Internal because EmployeeTypeDeleted is also marked as internal.
/// </summary>
internal class EmployeeTypeDeletedHandler(ILogger<EmployeeTypeDeletedHandler> logger, IEmailSender emailSender) : INotificationHandler<EmployeeTypeDeletedEvent>
{
  public async Task Handle(EmployeeTypeDeletedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Contributed Deleted event for {EmployeeTypeId}", domainEvent.EmployeeTypeId);

    await emailSender.SendEmailAsync("to@test.com",
                                     "from@test.com",
                                     "EmployeeType Deleted",
                                     $"EmployeeType with id {domainEvent.EmployeeTypeId} was deleted.");
  }
}
