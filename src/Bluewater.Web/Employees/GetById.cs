using Ardalis.Result;
using Bluewater.UseCases.Employees.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Employees;

/// <summary>
/// Retrieve a single employee by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetEmployeeByIdRequest, EmployeeRecord>
{
  public override void Configure()
  {
    Get(GetEmployeeByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetEmployeeByIdRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetEmployeeQuery(req.EmployeeId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = EmployeeMapper.ToRecord(result.Value);
    }
  }
}
