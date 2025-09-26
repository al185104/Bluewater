using Ardalis.Result;
using Bluewater.UseCases.ServiceCharges.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.ServiceCharges;

/// <summary>
/// Deletes a service charge entry.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteServiceChargeRequest>
{
  public override void Configure()
  {
    Delete(DeleteServiceChargeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteServiceChargeRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteServiceChargeCommand(request.ServiceChargeId), cancellationToken);

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
