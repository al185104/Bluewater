using Ardalis.Result;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;

/// <summary>
/// Update an existing timesheet entry.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateTimesheetRequest, UpdateTimesheetResponse>
{
  public override void Configure()
  {
    Put(UpdateTimesheetRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateTimesheetRequest request, CancellationToken cancellationToken)
  {
    var command = new UpdateTimesheetCommand(
      request.Id,
      request.EmployeeId,
      request.TimeIn1,
      request.TimeOut1,
      request.TimeIn2,
      request.TimeOut2,
      request.EntryDate,
      request.IsLocked);

    Result<TimesheetDTO> result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateTimesheetResponse(TimesheetMapper.ToRecord(result.Value));
    }
  }
}
