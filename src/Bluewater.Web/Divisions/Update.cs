using Ardalis.Result;
using Bluewater.UseCases.Divisions.Get;
using Bluewater.UseCases.Divisions.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Divisions;
/// <summary>
/// Update an existing Division
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateDivisionRequest, UpdateDivisionResponse>
{
  public override void Configure()
  {
    Put(UpdateDivisionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateDivisionRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateDivisionCommand(req.Id, req.Name!, req.Description), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var query = new GetDivisionQuery(req.Id);

    var queryResult = await _mediator.Send(query, ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateDivisionResponse(new DivisionRecord(dto.Id, dto.Name, dto.Description));
      return;
    } 
  }
}
