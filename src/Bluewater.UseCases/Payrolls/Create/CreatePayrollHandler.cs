using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayrollAggregate;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Bluewater.UseCases.Forms.Deductions;
using Bluewater.UseCases.Forms.Deductions.List;
using Bluewater.UseCases.Forms.Deductions.Update;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Payrolls.Create;

public class CreatePayrollHandler(IRepository<Payroll> _repository, IServiceScopeFactory serviceScopeFactory) : ICommandHandler<CreatePayrollCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreatePayrollCommand request, CancellationToken cancellationToken)
  {
    var newPayroll = new Payroll(request.employeeId, request.Date, request.grossPayAmount, request.netAmount, request.basicPayAmount, request.sssAmount, request.sssERAmount, request.pagibigAmount, request.pagibigERAmount, request.philhealthAmount, request.philhealthERAmount, request.restDayAmount, request.restDayHrs, request.regularHolidayAmount, request.regularHolidayHrs, request.specialHolidayAmount, request.specialHolidayHrs, request.overtimeAmount, request.overtimeHrs, request.nightDiffAmount, request.nightDiffHrs, request.nightDiffOvertimeAmount, request.nightDiffOvertimeHrs, request.nightDiffRegularHolidayAmount, request.nightDiffRegularHolidayHrs, request.nightDiffSpecialHolidayAmount, request.nightDiffSpecialHolidayHrs, request.overtimeRestDayAmount, request.overtimeRestDayHrs, request.overtimeRegularHolidayAmount, request.overtimeRegularHolidayHrs, request.overtimeSpecialHolidayAmount, request.overtimeSpecialHolidayHrs, request.unionDues, request.absences, request.absencesAmount, request.leaves, request.leavesAmount, request.lates, request.latesAmount, request.undertime, request.undertimeAmount, request.overbreak, request.overbreakAmount, request.svcCharge, request.costOfLivingAllowanceAmount, request.monthlyAllowanceAmount, request.salaryUnderpaymentAmount, request.refundAbsencesAmount, request.refundUndertimeAmount, request.refundOvertimeAmount, request.laborHoursIncome, request.laborHrs, request.taxDeductions, request.totalConstantDeductions, request.totalLoanDeductions, request.totalDeductions);
    var createdItem = await _repository.AddAsync(newPayroll, cancellationToken);
    
    // if there's current loan deductables in this current payroll, then update those deduction to dedcut the monthly from the total.
    if(createdItem != null && request.totalLoanDeductions > 0)
    {
      List<DeductionDTO> deductions = new();
      using (var scope = serviceScopeFactory.CreateScope()) {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          // only process the approved deduction forms
          var ret = await mediator.Send(new ListDeductionByEmpIdDatesQuery(null, null, request.employeeId, ApplicationStatusDTO.Approved, request.Date));
          if (ret.IsSuccess)
            deductions = ret.Value.ToList();
      }

      if(deductions != null && deductions.Any()) {
        foreach(var deduction in deductions) {
          using (var scope = serviceScopeFactory.CreateScope()) {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new UpdateDeductionCommand(deduction.Id, deduction.EmpId!.Value, deduction.Type!.Value, deduction.TotalAmount!.Value, deduction.MonthlyAmortization!.Value, 
            deduction.RemainingBalance!.Value - deduction.MonthlyAmortization!.Value, deduction.NoOfMonths!.Value, deduction.StartDate!.Value, deduction.EndDate!.Value, deduction.Remarks ?? string.Empty, deduction.Status!.Value));
          }
        }

      }
    }

    return createdItem?.Id ?? Guid.Empty;
  }
}
