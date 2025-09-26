using Ardalis.Result;
using Bluewater.UseCases.Timesheets.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;

/// <summary>
/// Delete a timesheet entry.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteTimesheetRequest>
{
  public override void Configure()
  {
    Delete(DeleteTimesheetRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteTimesheetRequest request, CancellationToken cancellationToken)
  {
    Result result = await _mediator.Send(new DeleteTimesheetCommand(request.TimesheetId), cancellationToken);

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
