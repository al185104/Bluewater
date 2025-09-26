using Bluewater.UseCases.ServiceCharges.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.ServiceCharges;

/// <summary>
/// Creates a service charge entry.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateServiceChargeRequest, CreateServiceChargeResponse>
{
  public override void Configure()
  {
    Post(CreateServiceChargeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateServiceChargeRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateServiceChargeCommand(request.Username, request.Amount, request.Date), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateServiceChargeResponse(new ServiceChargeRecord(result.Value, request.Username, request.Amount, request.Date));
    }
  }
}
