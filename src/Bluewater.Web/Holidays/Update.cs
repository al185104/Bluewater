using Ardalis.Result;
using Bluewater.UseCases.Holidays.Get;
using Bluewater.UseCases.Holidays.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Holidays;

/// <summary>
/// Update an existing holiday.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateHolidayRequest, UpdateHolidayResponse>
{
  public override void Configure()
  {
    Put(UpdateHolidayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateHolidayRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new UpdateHolidayCommand(req.Id, req.Name!, req.Description, req.Date, req.IsRegular), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    var queryResult = await _mediator.Send(new GetHolidayQuery(req.Id), ct);

    if (queryResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (queryResult.IsSuccess)
    {
      var dto = queryResult.Value;
      Response = new UpdateHolidayResponse(new HolidayRecord(dto.Id, dto.Name, dto.Description, dto.Date, dto.IsRegular));
    }
  }
}
