using Ardalis.Result;
using Bluewater.UseCases.EmployeeTypes.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.EmployeeTypes;

/// <summary>
/// Delete an employee type.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteEmployeeTypeRequest>
{
  public override void Configure()
  {
    Delete(DeleteEmployeeTypeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteEmployeeTypeRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteEmployeeTypeCommand(req.EmployeeTypeId), ct);

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
