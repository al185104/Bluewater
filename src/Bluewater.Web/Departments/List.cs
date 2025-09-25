using Ardalis.Result;
using Bluewater.UseCases.Departments.List;
using Bluewater.UseCases.Departments;
using Bluewater.Web.Departments;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Departments;
/// <summary>
/// List all Departments
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<DepartmentListResponse>
{
  public override void Configure()
  {
    Get("/Departments");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<DepartmentDTO>> result = await _mediator.Send(new ListDepartmentsQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new DepartmentListResponse
      {
        Departments = result.Value
          .Select(c => new DepartmentRecord(c.Id, c.Name, c.Description, c.DivisionId))
          .ToList()
      };
    }
  }
}
