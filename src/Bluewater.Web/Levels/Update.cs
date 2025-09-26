using Ardalis.Result;
using Bluewater.UseCases.Levels.Get;
using Bluewater.UseCases.Levels.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Levels;

/// <summary>
/// Update an existing employee level.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateLevelRequest, UpdateLevelResponse>
{
  public override void Configure()
  {
    Put(UpdateLevelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateLevelRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateLevelCommand(req.Id, req.Name!, req.Value!, req.IsActive), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var queryResult = await _mediator.Send(new GetLevelQuery(req.Id), ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateLevelResponse(new LevelRecord(dto.Id, dto.Name, dto.Value, dto.IsActive));
    }
  }
}
