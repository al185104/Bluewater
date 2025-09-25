using Ardalis.Result;
using Bluewater.UseCases.Positions.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Positions;

/// <summary>
/// Delete a Position
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeletePositionRequest>
{
  public override void Configure()
  {
    Delete(DeletePositionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeletePositionRequest request, CancellationToken cancellationToken)
  {
    var command = new DeletePositionCommand(request.PositionId);

    var result = await _mediator.Send(command, cancellationToken);

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
