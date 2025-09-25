using Ardalis.Result;
using Bluewater.UseCases.Chargings.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Chargings;

/// <summary>
/// Delete a charging definition.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteChargingRequest>
{
  public override void Configure()
  {
    Delete(DeleteChargingRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteChargingRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteChargingCommand(req.ChargingId), ct);

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
