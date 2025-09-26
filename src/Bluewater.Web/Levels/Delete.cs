using Ardalis.Result;
using Bluewater.UseCases.Levels.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Levels;

/// <summary>
/// Delete an employee level.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteLevelRequest>
{
  public override void Configure()
  {
    Delete(DeleteLevelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteLevelRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteLevelCommand(req.LevelId), ct);

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
