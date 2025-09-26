using Ardalis.Result;
using Bluewater.UseCases.Holidays.Get;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Holidays;

/// <summary>
/// Get a holiday by ID.
/// </summary>
/// <param name="_mediator"></param>
public class GetById(IMediator _mediator) : Endpoint<GetHolidayByIdRequest, HolidayRecord>
{
  public override void Configure()
  {
    Get(GetHolidayByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetHolidayByIdRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetHolidayQuery(req.HolidayId), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (result.IsSuccess)
    {
      var dto = result.Value;
      Response = new HolidayRecord(dto.Id, dto.Name, dto.Description, dto.Date, dto.IsRegular);
    }
  }
}
