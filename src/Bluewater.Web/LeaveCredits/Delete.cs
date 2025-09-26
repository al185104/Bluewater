using Ardalis.Result;
using Bluewater.UseCases.LeaveCredits.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.LeaveCredits;

/// <summary>
/// Delete a leave credit.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteLeaveCreditRequest>
{
  public override void Configure()
  {
    Delete(DeleteLeaveCreditRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteLeaveCreditRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteLeaveCreditCommand(req.LeaveCreditId), ct);

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
