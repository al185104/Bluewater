using Bluewater.UseCases.Shifts.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Shifts;

/// <summary>
/// Creates a new Shift
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateShiftRequest, CreateShiftResponse>
{
  public override void Configure()
  {
    Post(CreateShiftRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.ExampleRequest = new CreateShiftRequest
      {
        Name = "Morning Shift",
        ShiftStartTime = new TimeOnly(8, 0),
        ShiftBreakTime = new TimeOnly(12, 0),
        ShiftBreakEndTime = new TimeOnly(13, 0),
        ShiftEndTime = new TimeOnly(17, 0),
        BreakHours = 1m
      };
    });
  }

  public override async Task HandleAsync(CreateShiftRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreateShiftCommand(
        request.Name!,
        request.ShiftStartTime,
        request.ShiftBreakTime,
        request.ShiftBreakEndTime,
        request.ShiftEndTime,
        request.BreakHours
      ),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateShiftResponse(
        result.Value,
        request.Name!,
        request.ShiftStartTime,
        request.ShiftBreakTime,
        request.ShiftBreakEndTime,
        request.ShiftEndTime,
        request.BreakHours
      );
      return;
    }
  }
}
