using Ardalis.Result;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Payrolls;
using Bluewater.UseCases.Payrolls.List;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Payrolls;

/// <summary>
/// Lists payroll details for the specified filters.
/// </summary>
/// <param name="_mediator"></param>
public class List(IMediator _mediator) : Endpoint<PayrollListRequest, PayrollListResponse>
{
  public override void Configure()
  {
    Get("/Payrolls");
    AllowAnonymous();
  }

  public override async Task HandleAsync(PayrollListRequest request, CancellationToken cancellationToken)
  {
    Result<UseCases.Common.PagedResult<PayrollDTO>> result = await _mediator.Send(
      new ListPayrollQuery(request.Skip, request.Take, request.ChargingName, request.StartDate, request.EndDate, request.Tenant),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new PayrollListResponse
      {
        Payrolls = result.Value.Items.ToList(),
        TotalCount = result.Value.TotalCount
      };
    }
  }
}
