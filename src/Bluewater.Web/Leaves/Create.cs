using Bluewater.UseCases.Leaves.Create;
using Bluewater.UserCases.Forms.Enum;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Leaves;

/// <summary>
/// Create a new leave request.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateLeaveRequest, CreateLeaveResponse>
{
  public override void Configure()
  {
    Post(CreateLeaveRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateLeaveRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new CreateLeaveCommand(req.StartDate, req.EndDate, req.IsHalfDay, req.EmployeeId, req.LeaveCreditId),
      ct);

    if (result.IsSuccess)
    {
      Response = new CreateLeaveResponse(
        new LeaveRecord(
          result.Value,
          req.StartDate,
          req.EndDate,
          req.IsHalfDay,
          ApplicationStatusDTO.NotSet,
          req.EmployeeId,
          req.LeaveCreditId,
          string.Empty,
          string.Empty));
    }
  }
}
