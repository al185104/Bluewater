using Ardalis.Result;
using Bluewater.UseCases.Positions.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Positions;

/// <summary>
/// Get a Position by Guid ID
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetPositionByIdRequest, PositionRecord>
{
  public override void Configure()
  {
    Get(GetPositionByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetPositionByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetPositionQuery(request.PositionId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new PositionRecord(result.Value.Id, result.Value.Name, result.Value.Description, result.Value.SectionId);
    }
  }
}
