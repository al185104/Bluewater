using Ardalis.Result;
using Bluewater.UseCases.Levels;
using Bluewater.UseCases.Levels.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Levels;

/// <summary>
/// List employee levels.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<LevelListResponse>
{
  public override void Configure()
  {
    Get("/Levels");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<LevelDTO>> result = await _mediator.Send(new ListLevelQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new LevelListResponse
      {
        Levels = result.Value
          .Select(l => new LevelRecord(l.Id, l.Name, l.Value, l.IsActive))
          .ToList()
      };
    }
  }
}
