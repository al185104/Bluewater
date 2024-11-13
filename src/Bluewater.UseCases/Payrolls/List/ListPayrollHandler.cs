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
using Bluewater.UseCases.Holidays;
using Bluewater.UseCases.Holidays.List;
using Bluewater.UseCases.Forms.Overtimes;
using Bluewater.UseCases.Forms.Overtimes.List;
using Bluewater.UserCases.Forms.Enum;
using Bluewater.UseCases.Forms.OtherEarnings;
using Bluewater.UseCases.Forms.OtherEarnings.List;
using Bluewater.UseCases.Forms.Deductions;
using Bluewater.UseCases.Forms.Deductions.List;
using Bluewater.UseCases.ServiceCharges;
using Bluewater.UseCases.ServiceCharges.List;

namespace Bluewater.UseCases.Payrolls.List;

internal class ListPayrollHandler(IRepository<Payroll> _repository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListPayrollQuery, Result<IEnumerable<PayrollDTO>>>
{
  public async Task<Result<IEnumerable<PayrollDTO>>> Handle(ListPayrollQuery request, CancellationToken cancellationToken)
  {
    List<(Guid, string, PayDTO?, string?, string?, 
    string?, string?, string?, string?, string?)> employees = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var ret = await mediator.Send(new ListEmployeeQuery(null, null));
      if (ret.IsSuccess)
        employees = ret.Value
        .Where(c => 
          string.IsNullOrEmpty(request.chargingName) || 
          (!string.IsNullOrEmpty(c.Charging) && request.chargingName.Equals(c.Charging, StringComparison.InvariantCultureIgnoreCase))
        )
        .Select(s => (s.Id, $"{s.LastName}, {s.FirstName}", s.Pay, s.Type, s.User?.Username, 
          s.Division, s.Department, s.Section, s.Position, s.Charging)).ToList();
    }

    List<AllAttendancesDTO> attendances = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListAllAttendancesQuery(null, null, request.start, request.end));
        if (ret.IsSuccess)
            attendances = ret.Value.ToList();
    }

    // get all holidays by dates
    List<HolidayDTO> holidays = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListHolidayByDatesQuery(null, null, request.start, request.end));
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

    // get all other earnings.
    List<OtherEarningDTO> otherEarnings = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListOtherEarningsByDatesQuery(null, null, request.start, request.end));
        if (ret.IsSuccess)
            otherEarnings = ret.Value.ToList();
    }

    // get all deductions
    List<DeductionDTO> deductions = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListDeductionByEmpIdDatesQuery(null, null, null, ApplicationStatusDTO.Approved, request.end));
        if (ret.IsSuccess)
            deductions = ret.Value.ToList();
    }

    // get all service charges
    List<ServiceChargeDTO> serviceCharges = new();
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListServiceChargeQuery(null, null, request.end));
        if (ret.IsSuccess)
            serviceCharges = ret.Value.ToList();
    }
    
    // find the holidays that are between the start and end date
    var regularHolidayDates = holidays.Where(s => DateOnly.FromDateTime(s.Date) >= request.start && DateOnly.FromDateTime(s.Date) <= request.end && s.IsRegular).Select(i => DateOnly.FromDateTime(i.Date)).ToList();
    var specialHolidayDates = holidays.Where(s => DateOnly.FromDateTime(s.Date) >= request.start && DateOnly.FromDateTime(s.Date) <= request.end && !s.IsRegular).Select(i => DateOnly.FromDateTime(i.Date)).ToList();

    List<PayrollDTO> results = new();
    foreach (var emp in employees)
    {
      var empId = emp.Item1;
      var name = emp.Item2;
      var pay = emp.Item3;
      var type = emp.Item4;
      var username = emp.Item5;
      var division = emp.Item6;
      var department = emp.Item7;
      var section = emp.Item8;
      var position = emp.Item9;
      var charging = emp.Item10;

      var attendance = attendances.FirstOrDefault(s => s.EmployeeId == empId);
      if (attendance == null) continue;      

      var restDayHrs = attendance.Attendances.Where(s => s.Shift != null && s.Shift!.Name.Equals("Rest Day", StringComparison.InvariantCultureIgnoreCase)).Sum(i => i.WorkHrs);

      var regularHolidayHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.WorkHrs);
      var specialHolidayHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.WorkHrs);
      var overtimeApprovedHrs = overtimes.Where(s => s.EmpId == empId && s.Status == ApplicationStatusDTO.Approved).Sum(i => i.ApprovedHours);
      var nightOtHrs = CalculateNightDiffOvertimePremium(pay?.HourlyRate ?? 0, overtimes.Where(s => s.EmpId == empId && s.Status == ApplicationStatusDTO.Approved).ToList());
      var nightRegHolHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.NightShiftHours);
      var nightSpecHolHrs = attendance.Attendances.Where(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value)).Sum(i => i.NightShiftHours);
      
      var totalMonthlyAmortization = deductions.Where(s => s.EmpId == empId).Sum(i => i.MonthlyAmortization);

      var monal = otherEarnings.Where(s => s.EmpId == empId && s.EarningType == OtherEarningTypeDTO.MONAL).Sum(i => i.TotalAmount);
      var salun = otherEarnings.Where(s => s.EmpId == empId && s.EarningType == OtherEarningTypeDTO.SALUN).Sum(i => i.TotalAmount);
      var refabs = otherEarnings.Where(s => s.EmpId == empId && s.EarningType == OtherEarningTypeDTO.REFABS).Sum(i => i.TotalAmount);
      var refut = otherEarnings.Where(s => s.EmpId == empId && s.EarningType == OtherEarningTypeDTO.REFUT).Sum(i => i.TotalAmount);
      var refot = otherEarnings.Where(s => s.EmpId == empId && s.EarningType == OtherEarningTypeDTO.REFOT).Sum(i => i.TotalAmount);

      var otRestDayHrs = overtimes.Where(o => o.EmpId == empId && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.Shift != null && s.Shift!.Name.Equals("Rest Day", StringComparison.InvariantCultureIgnoreCase) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);

      var otRegHolHrs = overtimes.Where(o => o.EmpId == empId && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.EntryDate.HasValue && regularHolidayDates.Contains(s.EntryDate.Value) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);

      var otSpHolHrs = overtimes.Where(o => o.EmpId == empId && o.Status == ApplicationStatusDTO.Approved 
          && attendance.Attendances.Any(s => s.EntryDate.HasValue && specialHolidayDates.Contains(s.EntryDate.Value) 
          && o.StartDate.HasValue && o.EndDate.HasValue 
          && DateOnly.FromDateTime(o.StartDate.Value) >= s.EntryDate 
          && DateOnly.FromDateTime(o.EndDate.Value) <= s.EntryDate))
          .Sum(o => o.ApprovedHours);

      // absences is counted when the attendance has ShiftId but no TimesheetId, and the Shift.Name is not "Rest Day"
      var absences = attendance.Attendances.Where(s => s.Shift != null && s.ShiftId.HasValue && !s.TimesheetId.HasValue && !s.Shift!.Name.Equals("Rest Day", StringComparison.InvariantCultureIgnoreCase)).Count();

      // service charge by username
      var svcCharge = serviceCharges.FirstOrDefault(i => i.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

      var spec = new PayrollByIdSpec(empId, request.end);
      var payroll = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      
      if(payroll == null) {
        payroll = new Payroll();
        payroll.EmployeeId = empId;
        payroll.Date = request.end;
        payroll.SvcCharge = svcCharge?.Amount ?? 0;

        payroll.UpdatePayrollByAttendance(
          type: type ?? "Regular", 
          basicPay: pay?.BasicPay ?? 0, dailyRate: pay?.DailyRate ?? 0, hourlyRate: pay?.HourlyRate ?? 0, hdmfCon: pay?.HDMF_Con ?? 0, hdmfEr: pay?.HDMF_Er ?? 0, 
          totalWorkHours: attendance.TotalWorkHrs, totalLateHrs: attendance.TotalLateHrs, totalUnderHrs: attendance.TotalUnderHrs, attendance.TotalOverbreakHrs, attendance.TotalNightShiftHrs, totalLeaves: attendance.TotalLeaves,
          restDayHrs: restDayHrs ?? 0, regularHolidayHrs: regularHolidayHrs ?? 0, specialHolidayHrs: specialHolidayHrs ?? 0, overtimeHrs: overtimeApprovedHrs ?? 0, nightOtHrs: nightOtHrs, nightRegHolHrs: nightRegHolHrs ?? 0, nightSpecHolHrs: nightSpecHolHrs ?? 0, otRestDayHrs: otRestDayHrs ?? 0, otRegHolHrs: otRegHolHrs ?? 0, otSpHolHrs: otSpHolHrs ?? 0,
          /*other earnings*/ cola: pay?.Cola ?? 0, monal: monal ?? 0, salun: salun ?? 0, refabs: refabs ?? 0, refut: refut ?? 0, refot: refot ?? 0, 
          /*deductions*/ absences: absences, totalMonthlyAmortization: totalMonthlyAmortization ?? 0);
      }
      else {
        name = $"{payroll.Employee?.LastName}, {payroll.Employee?.FirstName}";
      }

      var _payRoll = new PayrollDTO(payroll!.Id, payroll.EmployeeId, name, payroll.Date, payroll.GrossPayAmount, payroll.NetAmount, payroll.BasicPayAmount, payroll.SSSAmount, payroll.SSSERAmount, payroll.PagibigAmount, payroll.PagibigERAmount, payroll.PhilhealthAmount, payroll.PhilhealthERAmount, payroll.RestDayAmount, payroll.RestDayHrs, payroll.RegularHolidayAmount, payroll.RegularHolidayHrs, payroll.SpecialHolidayAmount, payroll.SpecialHolidayHrs, payroll.OvertimeAmount, payroll.OvertimeHrs, payroll.NightDiffAmount, payroll.NightDiffHrs, payroll.NightDiffOvertimeAmount, payroll.NightDiffOvertimeHrs, payroll.NightDiffRegularHolidayAmount, payroll.NightDiffRegularHolidayHrs, payroll.NightDiffSpecialHolidayAmount, payroll.NightDiffSpecialHolidayHrs, payroll.OvertimeRestDayAmount, payroll.OvertimeRestDayHrs, payroll.OvertimeRegularHolidayAmount, payroll.OvertimeRegularHolidayHrs, payroll.OvertimeSpecialHolidayAmount, payroll.OvertimeSpecialHolidayHrs, payroll.UnionDues, payroll.Absences, payroll.AbsencesAmount, payroll.Leaves, payroll.LeavesAmount, payroll.Lates, payroll.LatesAmount, payroll.Undertime, payroll.UndertimeAmount, payroll.Overbreak, payroll.OverbreakAmount, payroll.SvcCharge, payroll.CostOfLivingAllowanceAmount, payroll.MonthlyAllowanceAmount, payroll.SalaryUnderpaymentAmount, payroll.RefundAbsencesAmount, payroll.RefundUndertimeAmount, payroll.RefundOvertimeAmount, payroll.LaborHoursIncome, payroll.LaborHrs, payroll.TaxDeductions, payroll.TotalConstantDeductions, payroll.TotalLoanDeductions, payroll.TotalDeductions);
      
      _payRoll.Division = division;
      _payRoll.Department = department;
      _payRoll.Section = section;
      _payRoll.Position = position;
      _payRoll.Charging = charging;

      results.Add(_payRoll);
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