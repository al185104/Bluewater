using Ardalis.Result;
using Bluewater.UseCases.Attendances.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Attendances;

/// <summary>
/// Retrieves a specific attendance entry by identifier.
/// </summary>
/// <param name="_mediator"></param>
public class Get(IMediator _mediator) : Endpoint<GetAttendanceByIdRequest, AttendanceRecord>
{
  public override void Configure()
  {
    Get(GetAttendanceByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetAttendanceByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetAttendanceQuery(request.AttendanceId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = AttendanceMapper.ToRecord(result.Value);
    }
  }
}
