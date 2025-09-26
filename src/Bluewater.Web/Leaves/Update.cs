using Ardalis.Result;
using Bluewater.UseCases.Leaves.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Leaves;

/// <summary>
/// Update an existing leave request.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateLeaveRequest, UpdateLeaveResponse>
{
  public override void Configure()
  {
    Put(UpdateLeaveRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateLeaveRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new UpdateLeaveCommand(req.Id, req.StartDate, req.EndDate, req.IsHalfDay, req.Status, req.EmployeeId, req.LeaveCreditId),
      ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      var dto = result.Value;
      Response = new UpdateLeaveResponse(
        new LeaveRecord(
          dto.Id,
          dto.StartDate,
          dto.EndDate,
          dto.IsHalfDay,
          dto.Status,
          dto.EmployeeId,
          dto.LeaveCreditId,
          dto.EmployeeName,
          dto.LeaveCreditName));
    }
  }
}
