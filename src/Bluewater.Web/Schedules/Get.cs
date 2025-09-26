using Ardalis.Result;
using Bluewater.UseCases.Schedules.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

/// <summary>
/// Retrieves a schedule entry by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetScheduleRequest, ScheduleRecord>
{
  public override void Configure()
  {
    Get(GetScheduleRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetScheduleRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetScheduleQuery(request.ScheduleId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = ScheduleMapper.ToRecord(result.Value);
    }
  }
}
