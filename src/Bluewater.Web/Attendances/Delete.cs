using Ardalis.Result;
using Bluewater.UseCases.Attendances.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Deletes an attendance entry.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteAttendanceRequest>
{
  public override void Configure()
  {
    Delete(DeleteAttendanceRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteAttendanceRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteAttendanceCommand(request.AttendanceId), cancellationToken);

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
