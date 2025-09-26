using Ardalis.Result;
using Bluewater.UseCases.Leaves;
using Bluewater.UseCases.Leaves.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Leaves;

/// <summary>
/// List leave requests.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<LeaveListRequest, LeaveListResponse>
{
  public override void Configure()
  {
    Get("/Leaves");
    AllowAnonymous();
  }

  public override async Task HandleAsync(LeaveListRequest req, CancellationToken ct)
  {
    Result<IEnumerable<LeaveDTO>> result = await _mediator.Send(new ListLeaveQuery(req.Skip, req.Take, req.Tenant), ct);

    if (result.IsSuccess)
    {
      Response = new LeaveListResponse
      {
        Leaves = result.Value
          .Select(l => new LeaveRecord(
            l.Id,
            l.StartDate,
            l.EndDate,
            l.IsHalfDay,
            l.Status,
            l.EmployeeId,
            l.LeaveCreditId,
            l.EmployeeName,
            l.LeaveCreditName))
          .ToList()
      };
    }
  }
}
