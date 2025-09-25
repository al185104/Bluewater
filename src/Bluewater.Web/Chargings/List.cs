using Ardalis.Result;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Chargings;

/// <summary>
/// List all charging definitions.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<ChargingListResponse>
{
  public override void Configure()
  {
    Get("/Chargings");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<ChargingDTO>> result = await _mediator.Send(new ListChargingQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new ChargingListResponse
      {
        Chargings = result.Value
          .Select(c => new ChargingRecord(c.Id, c.Name, c.Description, c.DepartmentId))
          .ToList()
      };
    }
  }
}
