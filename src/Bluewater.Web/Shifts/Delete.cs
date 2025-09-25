using Ardalis.Result;
using Bluewater.UseCases.Shifts.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Shifts;

/// <summary>
/// Delete a Shift
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteShiftRequest>
{
  public override void Configure()
  {
    Delete(DeleteShiftRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteShiftRequest request, CancellationToken cancellationToken)
  {
    var command = new DeleteShiftCommand(request.ShiftId);

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
