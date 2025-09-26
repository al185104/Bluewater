using Bluewater.UseCases.Payrolls.Create;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Payrolls;

/// <summary>
/// Creates a payroll entry.
/// </summary>
/// <param name="_mediator"></param>
public class Create(IMediator _mediator) : Endpoint<CreatePayrollRequest, CreatePayrollResponse>
{
  public override void Configure()
  {
    Post(CreatePayrollRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreatePayrollRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreatePayrollCommand(
      request.EmployeeId,
      request.Date,
      request.GrossPayAmount,
      request.NetAmount,
      request.BasicPayAmount,
      request.SssAmount,
      request.SssERAmount,
      request.PagibigAmount,
      request.PagibigERAmount,
      request.PhilhealthAmount,
      request.PhilhealthERAmount,
      request.RestDayAmount,
      request.RestDayHrs,
      request.RegularHolidayAmount,
      request.RegularHolidayHrs,
      request.SpecialHolidayAmount,
      request.SpecialHolidayHrs,
      request.OvertimeAmount,
      request.OvertimeHrs,
      request.NightDiffAmount,
      request.NightDiffHrs,
      request.NightDiffOvertimeAmount,
      request.NightDiffOvertimeHrs,
      request.NightDiffRegularHolidayAmount,
      request.NightDiffRegularHolidayHrs,
      request.NightDiffSpecialHolidayAmount,
      request.NightDiffSpecialHolidayHrs,
      request.OvertimeRestDayAmount,
      request.OvertimeRestDayHrs,
      request.OvertimeRegularHolidayAmount,
      request.OvertimeRegularHolidayHrs,
      request.OvertimeSpecialHolidayAmount,
      request.OvertimeSpecialHolidayHrs,
      request.UnionDues,
      request.Absences,
      request.AbsencesAmount,
      request.Leaves,
      request.LeavesAmount,
      request.Lates,
      request.LatesAmount,
      request.Undertime,
      request.UndertimeAmount,
      request.Overbreak,
      request.OverbreakAmount,
      request.SvcCharge,
      request.CostOfLivingAllowanceAmount,
      request.MonthlyAllowanceAmount,
      request.SalaryUnderpaymentAmount,
      request.RefundAbsencesAmount,
      request.RefundUndertimeAmount,
      request.RefundOvertimeAmount,
      request.LaborHoursIncome,
      request.LaborHrs,
      request.TaxDeductions,
      request.TotalConstantDeductions,
      request.TotalLoanDeductions,
      request.TotalDeductions), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreatePayrollResponse(result.Value);
    }
  }
}
