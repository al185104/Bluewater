using Ardalis.Result;
using Bluewater.UseCases.Departments.Get;
using Bluewater.UseCases.Departments.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Departments;
/// <summary>
/// Update an existing Department
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateDepartmentRequest, UpdateDepartmentResponse>
{
  public override void Configure()
  {
    Put(UpdateDepartmentRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateDepartmentRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateDepartmentCommand(req.Id, req.Name!, req.Description, req.DivisionId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var query = new GetDepartmentQuery(req.Id);

    var queryResult = await _mediator.Send(query, ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateDepartmentResponse(new DepartmentRecord(dto.Id, dto.Name, dto.Description));
      return;
    }
  }
}
