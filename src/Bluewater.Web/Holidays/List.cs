using Ardalis.Result;
using Bluewater.UseCases.Holidays;
using Bluewater.UseCases.Holidays.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Holidays;

/// <summary>
/// List all holidays.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<HolidayListResponse>
{
  public override void Configure()
  {
    Get("/Holidays");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<HolidayDTO>> result = await _mediator.Send(new ListHolidayQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new HolidayListResponse
      {
        Holidays = result.Value
          .Select(h => new HolidayRecord(h.Id, h.Name, h.Description, h.Date, h.IsRegular))
          .ToList()
      };
    }
  }
}
