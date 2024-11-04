using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Bluewater.UseCases.Employees.List;
using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Attendances.List;
using Bluewater.UseCases.Pays;
using Bluewater.UseCases.EmployeeTypes;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Bluewater.UseCases.Holidays;
using Bluewater.UseCases.Holidays.List;
using Bluewater.UseCases.Forms.Overtimes;
using Bluewater.UseCases.Forms.Overtimes.List;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Payrolls.List;

internal class ListPayrollHandler(IRepository<Payroll> _repository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListPayrollQuery, Result<IEnumerable<PayrollDTO>>>
{
  public async Task<Result<IEnumerable<PayrollDTO>>> Handle(ListPayrollQuery request, CancellationToken cancellationToken)
  {
    List<(Guid, PayDTO?, string?)> employees = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var ret = await mediator.Send(new ListEmployeeQuery(null, null));
      if (ret.IsSuccess)
        employees = ret.Value.Select(s => (s.Id, s.Pay, s.Type)).ToList();
    }

    List<AllAttendancesDTO> attendances = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListAllAttendancesQuery(null, null, request.start, request.end));
        if (ret.IsSuccess)
            attendances = ret.Value.ToList();
    }

    // get all holidays
    List<HolidayDTO> holidays = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListHolidayQuery(null, null));
        if (ret.IsSuccess)
            holidays = ret.Value.ToList();
    }

    // get all overtimes
    List<OvertimeDTO> overtimes = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListOvertimeByDatesQuery(null, null, request.start, request.end));
        if (ret.IsSuccess)
            overtimes = ret.Value.ToList();
    }

    // find the holidays that are between the start and end date
    var regularHolidayDates = holidays.Where(s => DateOnly.FromDateTime(s.Date) >= request.start && DateOnly.FromDateTime(s.Date) <= request.end && s.IsRegular).Select(i => DateOnly.FromDateTime(i.Date)).ToList();
    var specialHolidayDates = holidays.Where(s => DateOnly.FromDateTime(s.Date) >= request.start && DateOnly.FromDateTime(s.Date) <= request.end && !s.IsRegular).Select(i => DateOnly.FromDateTime(i.Date)).ToList();

    List<PayrollDTO> results = new();
    foreach (var emp in employees)
    {
      var id = emp.Item1;
      var pay = emp.Item2;
      var type = emp.Item3;

      var attendance = attendances.FirstOrDefault(s => s.EmployeeId == id);
      if (attendance == null) continue;

      var restDayHrs = attendance.Attendances.Where(s => s.Shift!.Name.Equals("Rest Day", StringComparison.InvariantCultureIgnoreCase)).Sum(i => i.WorkHrs);
      var regularHolidayHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.WorkHrs);
      var specialHolidayHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.WorkHrs);
      var overtimeApprovedHrs = overtimes.Where(s => s.EmpId == id && s.Status == ApplicationStatusDTO.Approved).Sum(i => i.ApprovedHours);
      var nightOtHrs = CalculateNightDiffOvertimePremium(pay?.HourlyRate ?? 0, overtimes.Where(s => s.EmpId == id && s.Status == ApplicationStatusDTO.Approved).ToList());
      var nightRegHolHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.NightShiftHours);
      var nightSpecHolHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.NightShiftHours);
      
      var otRestDayHrs = overtimes.Where(o => o.EmpId == id && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.Shift!.Name.Equals("Rest Day", StringComparison.InvariantCultureIgnoreCase) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);

      var otRegHolHrs = overtimes.Where(o => o.EmpId == id && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);

      var otSpHolHrs = overtimes.Where(o => o.EmpId == id && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);



      var spec = new PayrollByIdSpec(id);
      var payroll = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      
      if(payroll == null) {
        payroll = new Payroll(id);

        payroll.UpdatePayrollByAttendance(
          type: type ?? "Regular", 
          basicPay: pay?.BasicPay ?? 0, dailyRate: pay?.DailyRate ?? 0, hourlyRate: pay?.HourlyRate ?? 0, hdmfCon: pay?.HDMF_Con ?? 0, hdmfEr: pay?.HDMF_Er ?? 0, 
          totalWorkHours: attendance.TotalWorkHrs, totalLateHrs: attendance.TotalLateHrs, totalUnderHrs: attendance.TotalUnderHrs, attendance.TotalOverbreakHrs, attendance.TotalNightShiftHrs, totalLeaves: attendance.TotalLeaves,
          restDayHrs: restDayHrs ?? 0, regularHolidayHrs: regularHolidayHrs ?? 0, specialHolidayHrs: specialHolidayHrs ?? 0, overtimeHrs: overtimeApprovedHrs ?? 0, nightOtHrs: nightOtHrs, nightRegHolHrs: nightRegHolHrs ?? 0, nightSpecHolHrs: nightSpecHolHrs ?? 0, otRestDayHrs: otRestDayHrs ?? 0, otRegHolHrs: otRegHolHrs ?? 0, otSpHolHrs: otSpHolHrs ?? 0);
      }


        results.Add(new PayrollDTO(payroll!.Id, payroll.EmployeeId, $"{payroll.Employee?.LastName}, {payroll.Employee?.FirstName}", payroll.Date, payroll.GrossPayAmount, payroll.BasicPayAmount, payroll.SSSAmount, payroll.SSSERAmount, payroll.PagibigAmount, payroll.PagibigERAmount, payroll.PhilhealthAmount, payroll.PhilhealthERAmount, payroll.RestDayAmount, payroll.RestDayHrs, payroll.RegularHolidayAmount, payroll.RegularHolidayHrs, payroll.SpecialHolidayAmount, payroll.SpecialHolidayHrs, payroll.OvertimeAmount, payroll.OvertimeHrs, payroll.NightDiffAmount, payroll.NightDiffHrs, payroll.NightDiffOvertimeAmount, payroll.NightDiffOvertimeHrs, payroll.NightDiffRegularHolidayAmount, payroll.NightDiffRegularHolidayHrs, payroll.NightDiffSpecialHolidayAmount, payroll.NightDiffSpecialHolidayHrs, payroll.OvertimeRestDayAmount, payroll.OvertimeRestDayHrs, payroll.OvertimeRegularHolidayAmount, payroll.OvertimeRegularHolidayHrs, payroll.OvertimeSpecialHolidayAmount, payroll.OvertimeSpecialHolidayHrs, payroll.UnionDues, payroll.Absences, payroll.AbsencesAmount, payroll.Leaves, payroll.LeavesAmount, payroll.Lates, payroll.LatesAmount, payroll.Undertime, payroll.UndertimeAmount, payroll.Overbreak, payroll.OverbreakAmount, payroll.SvcCharge, payroll.CostOfLivingAllowanceAmount, payroll.MonthlyAllowanceAmount, payroll.SalaryUnderpaymentAmount, payroll.RefundAbsencesAmount, payroll.RefundUndertimeAmount, payroll.RefundOvertimeAmount, payroll.LaborHoursIncome, payroll.LaborHrs, payroll.TaxDeductions, payroll.TotalConstantDeductions, payroll.TotalLoanDeductions, payroll.TotalDeductions));
    }
    return Result.Success(results.AsEnumerable());
  }

  private decimal CalculateNightDiffOvertimePremium(decimal hourlyRate, List<OvertimeDTO> overtimes)
  {
      // check how many hours are between 10pm to 6am
      var nightOtHours = 0m;
      foreach (var ot in overtimes)
      {
        if(ot.StartDate == null || ot.EndDate == null) continue;

        var start = TimeOnly.FromDateTime(ot.StartDate.Value);
        var end = TimeOnly.FromDateTime(ot.EndDate.Value);
        decimal totalHours = 0m;
        if (start.Hour >= 22 || end.Hour <= 6)
        {
          if(end < start)
            totalHours = Math.Round((decimal)(end.Add(new TimeSpan(24,0,0)) - start).TotalHours, 2);
          else
            totalHours = Math.Round((decimal)(end - start).TotalHours, 2);
          
          nightOtHours += totalHours;
        }
      }
      
      return nightOtHours;
  }
}