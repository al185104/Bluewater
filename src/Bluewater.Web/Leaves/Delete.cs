using Ardalis.Result;
using Bluewater.UseCases.Leaves.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Leaves;

/// <summary>
/// Delete a leave request.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteLeaveRequest>
{
  public override void Configure()
  {
    Delete(DeleteLeaveRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteLeaveRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteLeaveCommand(req.LeaveId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(ct);
    }
  }
}
