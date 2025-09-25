using Ardalis.Result;
using Bluewater.UseCases.EmployeeTypes;
using Bluewater.UseCases.EmployeeTypes.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.EmployeeTypes;

/// <summary>
/// Update an existing employee type.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateEmployeeTypeRequest, UpdateEmployeeTypeResponse>
{
  public override void Configure()
  {
    Put(UpdateEmployeeTypeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateEmployeeTypeRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateEmployeeTypeCommand(req.Id, req.Name!, req.Value!, req.IsActive), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateEmployeeTypeResponse(new EmployeeTypeRecord(result.Value.Id, result.Value.Name, result.Value.Value, result.Value.IsActive));
    }
  }
}
