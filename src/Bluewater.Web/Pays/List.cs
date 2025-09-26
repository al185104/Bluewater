using Ardalis.Result;
using Bluewater.UseCases.Pays;
using Bluewater.UseCases.Pays.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Pays;

/// <summary>
/// Lists pay templates.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<PayListRequest, PayListResponse>
{
  public override void Configure()
  {
    Get("/Pays");
    AllowAnonymous();
  }

  public override async Task HandleAsync(PayListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<PayDTO>> result = await _mediator.Send(new ListPayQuery(request.Skip, request.Take), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new PayListResponse
      {
        Pays = result.Value.Select(PayMapper.ToRecord).ToList()
      };
    }
  }
}
