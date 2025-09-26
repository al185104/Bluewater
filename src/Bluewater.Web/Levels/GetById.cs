using Ardalis.Result;
using Bluewater.UseCases.Levels.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Levels;

/// <summary>
/// Get an employee level by ID.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetLevelByIdRequest, LevelRecord>
{
  public override void Configure()
  {
    Get(GetLevelByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetLevelByIdRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetLevelQuery(req.LevelId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      var dto = result.Value;
      Response = new LevelRecord(dto.Id, dto.Name, dto.Value, dto.IsActive);
    }
  }
}
