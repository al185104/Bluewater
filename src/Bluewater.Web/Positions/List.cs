using Ardalis.Result;
using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Positions.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Positions;

/// <summary>
/// List all Positions
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<PositionListResponse>
{
  public override void Configure()
  {
    Get("/Positions");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<PositionDTO>> result = await _mediator.Send(new ListPositionsQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new PositionListResponse
      {
        Positions = result.Value
          .Select(p => new PositionRecord(p.Id, p.Name, p.Description, p.SectionId))
          .ToList()
      };
    }
  }
}
