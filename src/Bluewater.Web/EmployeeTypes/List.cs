using Ardalis.Result;
using Bluewater.UseCases.EmployeeTypes;
using Bluewater.UseCases.EmployeeTypes.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.EmployeeTypes;

/// <summary>
/// List all employee types.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<EmployeeTypeListResponse>
{
  public override void Configure()
  {
    Get("/EmployeeTypes");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<EmployeeTypeDTO>> result = await _mediator.Send(new ListEmployeeTypeQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new EmployeeTypeListResponse
      {
        EmployeeTypes = result.Value
          .Select(t => new EmployeeTypeRecord(t.Id, t.Name, t.Value, t.IsActive))
          .ToList()
      };
    }
  }
}
