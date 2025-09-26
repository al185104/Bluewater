using Ardalis.Result;
using Bluewater.UseCases.Payrolls;
using Bluewater.UseCases.Payrolls.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Payrolls;

/// <summary>
/// Lists grouped payroll summaries.
/// </summary>
/// <param name="_mediator"></param>
public class ListGrouped(IMediator _mediator) : Endpoint<PayrollGroupedListRequest, PayrollGroupedListResponse>
{
  public override void Configure()
  {
    Get("/Payrolls/Grouped");
    AllowAnonymous();
  }

  public override async Task HandleAsync(PayrollGroupedListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<PayrollSummaryDTO>> result = await _mediator.Send(
      new ListGroupedPayrollQuery(request.Skip, request.Take),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new PayrollGroupedListResponse
      {
        Payrolls = result.Value.ToList()
      };
    }
  }
}
