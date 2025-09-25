using Ardalis.Result;
using Bluewater.UseCases.Shifts.Get;
using Bluewater.UseCases.Shifts.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Shifts;

/// <summary>
/// Update an existing Shift
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateShiftRequest, UpdateShiftResponse>
{
  public override void Configure()
  {
    Put(UpdateShiftRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateShiftRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new UpdateShiftCommand(
        req.Id,
        req.Name!,
        req.ShiftStartTime,
        req.ShiftBreakTime,
        req.ShiftBreakEndTime,
        req.ShiftEndTime,
        req.BreakHours
      ),
      ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var query = new GetShiftQuery(req.Id);

    var queryResult = await _mediator.Send(query, ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateShiftResponse(
        new ShiftRecord(
          dto.Id,
          dto.Name,
          dto.ShiftStartTime,
          dto.ShiftBreakTime,
          dto.ShiftBreakEndTime,
          dto.ShiftEndTime,
          dto.BreakHours
        )
      );
      return;
    }
  }
}
