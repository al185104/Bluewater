using Ardalis.Result;
using Bluewater.UseCases.Divisions.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Divisions;
/// <summary>
/// Get a Division by Guid ID
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetDivisionByIdRequest, DivisionRecord>
{
  public override void Configure()
  {
    Get(GetDivisionByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetDivisionByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetDivisionQuery(request.DivisionId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new DivisionRecord(result.Value.Id, result.Value.Name, result.Value.Description);
    }
  }
}
