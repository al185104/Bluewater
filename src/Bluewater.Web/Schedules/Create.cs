using Bluewater.UseCases.Schedules.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

/// <summary>
/// Creates a schedule entry.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateScheduleRequest, CreateScheduleResponse>
{
  public override void Configure()
  {
    Post(CreateScheduleRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateScheduleRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateScheduleCommand(request.EmployeeId, request.ShiftId, request.ScheduleDate, request.IsDefault), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateScheduleResponse(result.Value);
    }
  }
}
