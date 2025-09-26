using Ardalis.Result;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;

/// <summary>
/// Retrieve an individual timesheet record.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetTimesheetRequest, GetTimesheetResponse>
{
  public override void Configure()
  {
    Get(GetTimesheetRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetTimesheetRequest request, CancellationToken cancellationToken)
  {
    Result<TimesheetDTO> result = await _mediator.Send(new GetTimesheetQuery(request.TimesheetId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new GetTimesheetResponse(TimesheetMapper.ToRecord(result.Value));
    }
  }
}
