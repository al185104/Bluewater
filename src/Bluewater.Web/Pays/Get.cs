using Ardalis.Result;
using Bluewater.UseCases.Pays.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Pays;

/// <summary>
/// Retrieves a pay template by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetPayRequest, PayRecord>
{
  public override void Configure()
  {
    Get(GetPayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetPayRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetPayQuery(request.PayId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = PayMapper.ToRecord(result.Value);
    }
  }
}
