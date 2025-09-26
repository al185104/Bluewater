using Ardalis.Result;
using Bluewater.UseCases.Pays.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Pays;

/// <summary>
/// Updates a pay template.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdatePayRequest, UpdatePayResponse>
{
  public override void Configure()
  {
    Put(UpdatePayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdatePayRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdatePayCommand(request.PayId, request.BasicPay, request.DailyRate, request.HourlyRate, request.HdmfCon, request.HdmfEr, request.Cola),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdatePayResponse(PayMapper.ToRecord(result.Value));
    }
  }
}
