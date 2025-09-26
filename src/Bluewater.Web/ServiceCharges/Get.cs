using Ardalis.Result;
using Bluewater.UseCases.ServiceCharges.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.ServiceCharges;

/// <summary>
/// Retrieves a service charge entry by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetServiceChargeRequest, ServiceChargeRecord>
{
  public override void Configure()
  {
    Get(GetServiceChargeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetServiceChargeRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetServiceChargeQuery(request.ServiceChargeId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = ServiceChargeMapper.ToRecord(result.Value);
    }
  }
}
