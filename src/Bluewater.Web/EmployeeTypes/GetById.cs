using Ardalis.Result;
using Bluewater.UseCases.EmployeeTypes.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.EmployeeTypes;

/// <summary>
/// Retrieve a specific employee type by its identifier.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetEmployeeTypeByIdRequest, EmployeeTypeRecord>
{
  public override void Configure()
  {
    Get(GetEmployeeTypeByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetEmployeeTypeByIdRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetEmployeeTypeQuery(req.EmployeeTypeId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new EmployeeTypeRecord(result.Value.Id, result.Value.Name, result.Value.Value, result.Value.IsActive);
    }
  }
}
