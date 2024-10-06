using Ardalis.Result;
using Bluewater.UseCases.Divisions;
using Bluewater.UseCases.Divisions.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Divisions;
/// <summary>
/// List all Divisions
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<DivisionListResponse>
{
  public override void Configure()
  {
    Get("/Divisions");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<DivisionDTO>> result = await _mediator.Send(new ListDivisionsQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new DivisionListResponse
      {
        Divisions = result.Value.Select(d => new DivisionRecord(d.Id, d.Name, d.Description)).ToList()
      };
    }
  }
}
