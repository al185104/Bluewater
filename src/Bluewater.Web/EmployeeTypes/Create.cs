using Bluewater.UseCases.EmployeeTypes.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.EmployeeTypes;

/// <summary>
/// Create a new employee type.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateEmployeeTypeRequest, CreateEmployeeTypeResponse>
{
  public override void Configure()
  {
    Post(CreateEmployeeTypeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateEmployeeTypeRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateEmployeeTypeCommand(req.Name!, req.Value!, req.IsActive), ct);

    if (result.IsSuccess)
    {
      Response = new CreateEmployeeTypeResponse(result.Value, req.Name!, req.Value!, req.IsActive);
    }
  }
}
