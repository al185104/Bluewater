using Ardalis.Result;
using Bluewater.UseCases.ServiceCharges;
using Bluewater.UseCases.ServiceCharges.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.ServiceCharges;

/// <summary>
/// Lists service charge entries for a specific date.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<ServiceChargeListRequest, ServiceChargeListResponse>
{
  public override void Configure()
  {
    Get("/ServiceCharges");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ServiceChargeListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<ServiceChargeDTO>> result = await _mediator.Send(
      new ListServiceChargeQuery(request.Skip, request.Take, request.Date),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ServiceChargeListResponse
      {
        ServiceCharges = result.Value.Select(ServiceChargeMapper.ToRecord).ToList()
      };
    }
  }
}
