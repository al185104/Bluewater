using Ardalis.Result;
using Bluewater.UseCases.LeaveCredits;
using Bluewater.UseCases.LeaveCredits.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.LeaveCredits;

/// <summary>
/// List all leave credits.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : EndpointWithoutRequest<LeaveCreditListResponse>
{
  public override void Configure()
  {
    Get("/LeaveCredits");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    Result<IEnumerable<LeaveCreditDTO>> result = await _mediator.Send(new ListLeaveCreditQuery(null, null), ct);

    if (result.IsSuccess)
    {
      Response = new LeaveCreditListResponse
      {
        LeaveCredits = result.Value
          .Select(l => new LeaveCreditRecord(l.Id, l.Code, l.Description, l.DefaultCredits, l.SortOrder, l.IsLeaveWithPay, l.IsCanCarryOver))
          .ToList()
      };
    }
  }
}
