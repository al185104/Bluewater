using Ardalis.Result;
using Bluewater.UseCases.Divisions.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Divisions;
/// <summary>
/// Delete a Division
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteDivisionRequest>
{
  public override void Configure()
  {
    Delete(DeleteDivisionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    DeleteDivisionRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteDivisionCommand(request.DivisionId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    };
  }
}
