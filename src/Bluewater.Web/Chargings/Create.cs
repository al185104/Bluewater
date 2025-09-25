using Bluewater.UseCases.Chargings.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Chargings;

/// <summary>
/// Create a new charging definition.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateChargingRequest, CreateChargingResponse>
{
  public override void Configure()
  {
    Post(CreateChargingRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateChargingRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateChargingCommand(req.Name!, req.Description, req.DepartmentId), ct);

    if (result.IsSuccess)
    {
      Response = new CreateChargingResponse(result.Value, req.Name!, req.Description, req.DepartmentId);
    }
  }
}
