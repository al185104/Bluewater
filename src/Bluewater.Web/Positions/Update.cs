using Ardalis.Result;
using Bluewater.UseCases.Positions.Get;
using Bluewater.UseCases.Positions.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Positions;

/// <summary>
/// Update an existing Position
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdatePositionRequest, UpdatePositionResponse>
{
  public override void Configure()
  {
    Put(UpdatePositionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdatePositionRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdatePositionCommand(req.Id, req.Name!, req.Description, req.SectionId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var query = new GetPositionQuery(req.Id);

    var queryResult = await _mediator.Send(query, ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdatePositionResponse(new PositionRecord(dto.Id, dto.Name, dto.Description, dto.SectionId));
      return;
    }
  }
}
