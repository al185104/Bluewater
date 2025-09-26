using Ardalis.Result;
using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Attendances.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Lists attendance summaries for all employees within a charging and date range.
/// </summary>
/// <param name="_mediator"></param>
public class ListAll(IMediator _mediator) : Endpoint<AttendanceListAllRequest, AttendanceListAllResponse>
{
  public override void Configure()
  {
    Get("/Attendances/All");
    AllowAnonymous();
  }

  public override async Task HandleAsync(AttendanceListAllRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<AllAttendancesDTO>> result = await _mediator.Send(
      new ListAllAttendancesQuery(request.Skip, request.Take, request.Charging, request.StartDate, request.EndDate, request.Tenant),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new AttendanceListAllResponse
      {
        Employees = result.Value.Select(AttendanceMapper.ToRecord).ToList()
      };
    }
  }
}
