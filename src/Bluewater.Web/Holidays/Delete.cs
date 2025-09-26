using Ardalis.Result;
using Bluewater.UseCases.Holidays.Delete;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Holidays;

/// <summary>
/// Delete a holiday.
/// </summary>
/// <param name="_mediator"></param>
public class Delete(IMediator _mediator) : Endpoint<DeleteHolidayRequest>
{
  public override void Configure()
  {
    Delete(DeleteHolidayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteHolidayRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteHolidayCommand(req.HolidayId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(ct);
    }
  }
}
