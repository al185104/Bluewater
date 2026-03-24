using Bluewater.UseCases.Forms.Deductions.Create;
using Bluewater.UserCases.Forms.Enum;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Deductions;

/// <summary>
/// Creates a new deduction.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreateDeductionRequest, CreateDeductionResponse>
{
  public override void Configure()
  {
    Post(CreateDeductionRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateDeductionRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreateDeductionCommand(
        request.EmpId,
        request.Type,
        request.TotalAmount,
        request.MonthlyAmortization,
        request.RemainingBalance,
        request.NoOfMonths,
        request.StartDate.HasValue ? DateOnly.FromDateTime(request.StartDate.Value) : null,
        request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value) : null,
        request.Remarks),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateDeductionResponse(
        new DeductionRecord(
          result.Value,
          request.EmpId,
          string.Empty,
          request.Type,
          request.TotalAmount,
          request.MonthlyAmortization,
          request.RemainingBalance,
          request.NoOfMonths,
          request.StartDate,
          request.EndDate,
          request.Remarks,
          ApplicationStatusDTO.NotSet));
    }
  }
}
