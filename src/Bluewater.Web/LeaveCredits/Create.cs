using Bluewater.UseCases.LeaveCredits.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.LeaveCredits;

/// <summary>
/// Create a new leave credit type.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateLeaveCreditRequest, CreateLeaveCreditResponse>
{
  public override void Configure()
  {
    Post(CreateLeaveCreditRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateLeaveCreditRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new CreateLeaveCreditCommand(
        req.Code!,
        req.Description ?? string.Empty,
        req.Credit,
        req.SortOrder,
        req.IsLeaveWithPay,
        req.IsCanCarryOver),
      ct);

    if (result.IsSuccess)
    {
      Response = new CreateLeaveCreditResponse(
        result.Value,
        req.Code!,
        req.Description ?? string.Empty,
        req.Credit ?? 0m,
        req.SortOrder ?? 0,
        req.IsLeaveWithPay,
        req.IsCanCarryOver);
    }
  }
}
