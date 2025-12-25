using Ardalis.Result;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Schedules;
using Bluewater.UseCases.Schedules.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Schedules;

/// <summary>
/// Lists schedules grouped by employee for a given charging and date range.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<ScheduleListRequest, ScheduleListResponse>
{
  public override void Configure()
  {
    Get("/Schedules");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ScheduleListRequest request, CancellationToken cancellationToken)
  {
    Result<PagedResult<EmployeeScheduleDTO>> result = await _mediator.Send(
      new ListScheduleQuery(request.Skip, request.Take, request.ChargingName, request.StartDate, request.EndDate, request.Tenant),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ScheduleListResponse
      {
        Employees = result.Value.Items.Select(ScheduleMapper.ToRecord).ToList(),
        TotalCount = result.Value.TotalCount
      };
    }
  }
}
