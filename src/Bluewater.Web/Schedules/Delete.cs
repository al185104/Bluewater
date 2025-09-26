using Ardalis.Result;
using Bluewater.UseCases.Schedules.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

/// <summary>
/// Deletes a schedule entry.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteScheduleRequest>
{
  public override void Configure()
  {
    Delete(DeleteScheduleRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteScheduleRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteScheduleCommand(request.ScheduleId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    }
  }
}
