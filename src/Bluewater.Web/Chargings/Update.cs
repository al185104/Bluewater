using Ardalis.Result;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Chargings;

/// <summary>
/// Update an existing charging definition.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateChargingRequest, UpdateChargingResponse>
{
  public override void Configure()
  {
    Put(UpdateChargingRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateChargingRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateChargingCommand(req.Id, req.Name!, req.Description, req.DepartmentId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateChargingResponse(new ChargingRecord(result.Value.Id, result.Value.Name, result.Value.Description, result.Value.DepartmentId));
    }
  }
}
