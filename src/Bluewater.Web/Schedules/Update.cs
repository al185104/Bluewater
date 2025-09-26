using Ardalis.Result;
using Bluewater.UseCases.Schedules.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

/// <summary>
/// Updates a schedule entry.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateScheduleRequest, UpdateScheduleResponse>
{
  public override void Configure()
  {
    Put(UpdateScheduleRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateScheduleRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateScheduleCommand(request.ScheduleId, request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateScheduleResponse(ScheduleMapper.ToRecord(result.Value));
    }
  }
}
