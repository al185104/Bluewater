using Ardalis.Result;
using Bluewater.UseCases.Employees.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Employees;

/// <summary>
/// Delete an existing employee.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteEmployeeRequest>
{
  public override void Configure()
  {
    Delete(DeleteEmployeeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteEmployeeRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteEmployeeCommand(req.EmployeeId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(ct);
    }
  }
}
