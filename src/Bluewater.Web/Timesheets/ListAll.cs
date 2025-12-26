using System.Linq;
using Ardalis.Result;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;

/// <summary>
/// Retrieve timesheet summaries for all employees within a tenant and date range.
/// </summary>
/// <param name="_mediator"></param>
public class ListAll(IMediator _mediator) : Endpoint<TimesheetListAllRequest, TimesheetListAllResponse>
{
  public override void Configure()
  {
    Get("/Timesheets/All");
    AllowAnonymous();
  }

  public override async Task HandleAsync(TimesheetListAllRequest request, CancellationToken cancellationToken)
  {
    Result<UseCases.Common.PagedResult<AllEmployeeTimesheetDTO>> result = await _mediator.Send(
      new ListAllTimesheetQuery(request.Skip, request.Take, request.Charging, request.StartDate, request.EndDate, request.Tenant),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new TimesheetListAllResponse
      {
        Employees = result.Value.Items.Select(TimesheetMapper.ToRecord).ToList(),
        TotalCount = result.Value.TotalCount
      };
    }
  }
}
