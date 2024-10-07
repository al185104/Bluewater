using Bluewater.Core.LevelAggregate.Events;
using Bluewater.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bluewater.Core.LevelAggregate.Handlers;
/// <summary>
/// NOTE: Internal because LevelDeleted is also marked as internal.
/// </summary>
internal class LevelDeletedHandler(ILogger<LevelDeletedHandler> logger, IEmailSender emailSender) : INotificationHandler<LevelDeletedEvent>
{
  public async Task Handle(LevelDeletedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Contributed Deleted event for {LevelId}", domainEvent.LevelId);

    await emailSender.SendEmailAsync("to@test.com",
                                     "from@test.com",
                                     "Level Deleted",
                                     $"Level with id {domainEvent.LevelId} was deleted.");
  }
}
