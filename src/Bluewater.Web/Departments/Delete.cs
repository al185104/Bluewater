using Ardalis.Result;
using Bluewater.UseCases.Departments.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Departments;
/// <summary>
/// Delete a Department
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteDepartmentRequest>
{
  public override void Configure()
  {
    Delete(DeleteDepartmentRequest.Route);
    AllowAnonymous();
  }


  public override async Task HandleAsync(DeleteDepartmentRequest request, CancellationToken cancellationToken)
  {
    var command = new DeleteDepartmentCommand(request.DepartmentId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    };
    // TODO: Handle other issues as needed
  }
}
