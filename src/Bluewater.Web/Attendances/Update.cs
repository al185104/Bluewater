using Ardalis.Result;
using Bluewater.UseCases.Attendances.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Updates an existing attendance entry identified by employee and date.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateAttendanceRequest, UpdateAttendanceResponse>
{
  public override void Configure()
  {
    Put(UpdateAttendanceRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateAttendanceRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateAttendanceCommand(
        request.EmployeeId,
        request.ShiftId,
        request.TimesheetId,
        request.LeaveId,
        request.EntryDate,
        request.IsLocked),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateAttendanceResponse(AttendanceMapper.ToRecord(result.Value));
    }
  }
}
