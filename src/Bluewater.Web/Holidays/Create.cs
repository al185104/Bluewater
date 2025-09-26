using Bluewater.UseCases.Holidays.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Holidays;

/// <summary>
/// Create a new holiday.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateHolidayRequest, CreateHolidayResponse>
{
  public override void Configure()
  {
    Post(CreateHolidayRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateHolidayRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateHolidayCommand(req.Name!, req.Description, req.Date, req.IsRegular), ct);

    if (result.IsSuccess)
    {
      Response = new CreateHolidayResponse(result.Value, req.Name!, req.Description, req.Date, req.IsRegular);
    }
  }
}
