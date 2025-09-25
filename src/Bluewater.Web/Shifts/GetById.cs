using Ardalis.Result;
using Bluewater.UseCases.Shifts.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Shifts;

/// <summary>
/// Get a Shift by Guid ID
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetShiftByIdRequest, ShiftRecord>
{
  public override void Configure()
  {
    Get(GetShiftByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetShiftByIdRequest request, CancellationToken cancellationToken)
  {
    var query = new GetShiftQuery(request.ShiftId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new ShiftRecord(
        result.Value.Id,
        result.Value.Name,
        result.Value.ShiftStartTime,
        result.Value.ShiftBreakTime,
        result.Value.ShiftBreakEndTime,
        result.Value.ShiftEndTime,
        result.Value.BreakHours
      );
    }
  }
}
