using Ardalis.Result;
using Bluewater.UseCases.Chargings.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Chargings;

/// <summary>
/// Retrieve a charging definition by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetChargingByIdRequest, ChargingRecord>
{
  public override void Configure()
  {
    Get(GetChargingByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetChargingByIdRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetChargingQuery(req.ChargingId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new ChargingRecord(result.Value.Id, result.Value.Name, result.Value.Description, result.Value.DepartmentId);
    }
  }
}
