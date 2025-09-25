using Ardalis.Result;
using Bluewater.UseCases.Departments.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Departments;
/// <summary>
/// Get a Department by Guid ID
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetDepartmentByIdRequest, DepartmentRecord>
{
  public override void Configure()
  {
    Get(GetDepartmentByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetDepartmentByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetDepartmentQuery(request.DepartmentId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new DepartmentRecord(result.Value.Id, result.Value.Name, result.Value.Description, result.Value.DivisionId);
    }
  }
}
