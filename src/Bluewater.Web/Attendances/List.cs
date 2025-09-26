using Ardalis.Result;
using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Attendances.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Lists attendance entries for a specific employee.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<AttendanceListRequest, AttendanceListResponse>
{
  public override void Configure()
  {
    Get("/Attendances");
    AllowAnonymous();
  }

  public override async Task HandleAsync(AttendanceListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<AttendanceDTO>> result = await _mediator.Send(
      new ListAttendanceQuery(request.Skip, request.Take, request.EmployeeId, request.StartDate, request.EndDate),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new AttendanceListResponse
      {
        Attendances = result.Value.Select(AttendanceMapper.ToRecord).ToList()
      };
    }
  }
}
