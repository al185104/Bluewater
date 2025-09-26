using Ardalis.Result;
using Bluewater.UseCases.Pays.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Pays;

/// <summary>
/// Deletes a pay template.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeletePayRequest>
{
  public override void Configure()
  {
    Delete(DeletePayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeletePayRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeletePayCommand(request.PayId), cancellationToken);

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
