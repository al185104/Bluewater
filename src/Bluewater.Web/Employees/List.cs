using Ardalis.Result;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Employees;

/// <summary>
/// List employees with optional paging parameters.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<ListEmployeeRequest, EmployeeListResponse>
{
  public override void Configure()
  {
    Get("/Employees");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ListEmployeeRequest req, CancellationToken ct)
  {
    Result<IEnumerable<EmployeeDTO>> result = await _mediator.Send(
      new ListEmployeeQuery(req.Skip, req.Take, req.Tenant),
      ct);

    if (result.IsSuccess)
    {
      Response = new EmployeeListResponse
      {
        Employees = result.Value
          .Select(EmployeeMapper.ToRecord)
          .ToList()
      };
    }
  }
}
