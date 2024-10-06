using Bluewater.UseCases.Departments.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Departments;
/// <summary>
/// Creates a new Department
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateDepartmentRequest, CreateDepartmentResponse>
{
  public override void Configure()
  {
    Post(CreateDepartmentRequest.Route);
    AllowAnonymous();
    Summary(s => { s.ExampleRequest = new CreateDepartmentRequest { Name = "Department Name", Description = "Department Description" }; });
  }

  public override async Task HandleAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateDepartmentCommand(request.Name!, request.Description, request.DivisionId), cancellationToken);
    if (result.IsSuccess)
    {
      Response = new CreateDepartmentResponse(result.Value, request.Name!, request.Description);
      return;
    }
  }
}
