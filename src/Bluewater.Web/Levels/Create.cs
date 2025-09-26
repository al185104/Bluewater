using Bluewater.UseCases.Levels.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Levels;

/// <summary>
/// Create a new employee level.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateLevelRequest, CreateLevelResponse>
{
  public override void Configure()
  {
    Post(CreateLevelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateLevelRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateLevelCommand(req.Name!, req.Value!, req.IsActive), ct);

    if (result.IsSuccess)
    {
      Response = new CreateLevelResponse(result.Value, req.Name!, req.Value!, req.IsActive);
    }
  }
}
