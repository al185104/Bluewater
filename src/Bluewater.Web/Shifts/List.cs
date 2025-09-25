using Ardalis.Result;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Shifts.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Shifts;

/// <summary>
/// List all Shifts
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<ShiftListResponse>
{
  public override void Configure()
  {
    Get("/Shifts");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<ShiftDTO>> result = await _mediator.Send(new ListShiftsQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ShiftListResponse
      {
        Shifts = result.Value
          .Select(s => new ShiftRecord(
            s.Id,
            s.Name,
            s.ShiftStartTime,
            s.ShiftBreakTime,
            s.ShiftBreakEndTime,
            s.ShiftEndTime,
            s.BreakHours))
          .ToList()
      };
    }
  }
}
