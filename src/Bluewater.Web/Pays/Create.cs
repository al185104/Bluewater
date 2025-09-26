using Bluewater.UseCases.Pays.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Pays;

/// <summary>
/// Creates a pay template.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreatePayRequest, CreatePayResponse>
{
  public override void Configure()
  {
    Post(CreatePayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreatePayRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreatePayCommand(request.BasicPay, request.DailyRate, request.HourlyRate, request.HdmfCon, request.HdmfEr), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreatePayResponse(new PayRecord(result.Value, request.BasicPay, request.DailyRate, request.HourlyRate, request.HdmfCon, request.HdmfEr, 0));
    }
  }
}
