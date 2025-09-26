using Bluewater.UseCases.Attendances.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Creates a new attendance entry.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateAttendanceRequest, CreateAttendanceResponse>
{
  public override void Configure()
  {
    Post(CreateAttendanceRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Creates a new attendance record.";
      s.Description = "Creates a new attendance record using the provided information.";
      s.ExampleRequest = new CreateAttendanceRequest
      {
        EmployeeId = Guid.NewGuid(),
        ShiftId = Guid.NewGuid(),
        TimesheetId = Guid.NewGuid(),
        EntryDate = DateOnly.FromDateTime(DateTime.Today),
        WorkHrs = 8,
        IsLocked = false
      };
    });
  }

  public override async Task HandleAsync(CreateAttendanceRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateAttendanceCommand(
      request.EmployeeId,
      request.ShiftId,
      request.TimesheetId,
      request.LeaveId,
      request.EntryDate,
      request.WorkHrs,
      request.LateHrs,
      request.UnderHrs,
      request.OverbreakHrs,
      request.NightShiftHrs,
      request.IsLocked);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateAttendanceResponse(
        new AttendanceRecord(
          result.Value,
          request.EmployeeId,
          request.ShiftId,
          request.TimesheetId,
          request.LeaveId,
          request.EntryDate,
          request.WorkHrs,
          request.LateHrs,
          request.UnderHrs,
          request.OverbreakHrs,
          request.NightShiftHrs,
          request.IsLocked,
          null,
          null));
    }
  }
}
