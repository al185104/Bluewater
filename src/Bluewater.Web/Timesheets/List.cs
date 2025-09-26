using Ardalis.Result;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;

/// <summary>
/// Retrieve timesheets for a specific employee over a date range.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<TimesheetListRequest, TimesheetListResponse>
{
  public override void Configure()
  {
    Get("/Timesheets");
    AllowAnonymous();
  }

  public override async Task HandleAsync(TimesheetListRequest request, CancellationToken cancellationToken)
  {
    Result<EmployeeTimesheetDTO> result = await _mediator.Send(
      new ListTimesheetQuery(request.Skip, request.Take, request.Name, request.StartDate, request.EndDate),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new TimesheetListResponse
      {
        Employee = TimesheetMapper.ToRecord(result.Value)
      };
    }
  }
}
