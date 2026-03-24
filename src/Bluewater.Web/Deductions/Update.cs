using Ardalis.Result;
using Bluewater.UseCases.Forms.Deductions.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Deductions;

/// <summary>
/// Updates an existing deduction.
/// </summary>
/// <param name="_mediator"></param>
public class Update(IMediator _mediator) : Endpoint<UpdateDeductionRequest, UpdateDeductionResponse>
{
  public override void Configure()
  {
    Put(UpdateDeductionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateDeductionRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateDeductionCommand(
        request.Id,
        request.EmpId,
        request.Type,
        request.TotalAmount,
        request.MonthlyAmortization,
        request.RemainingBalance,
        request.NoOfMonths,
        DateOnly.FromDateTime(request.StartDate),
        DateOnly.FromDateTime(request.EndDate),
        request.Remarks,
        request.Status),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new UpdateDeductionResponse(DeductionMapper.ToRecord(result.Value));
    }
  }
}
