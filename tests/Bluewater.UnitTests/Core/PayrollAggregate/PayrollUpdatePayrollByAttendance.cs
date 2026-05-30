using Bluewater.Core.PayrollAggregate;
using FluentAssertions;
using Xunit;

namespace Bluewater.UnitTests.Core.PayrollAggregate;

public class PayrollUpdatePayrollByAttendance
{
  [Fact]
  public void UpdatePayrollByAttendance_DoesNotDeductApprovedLeavesThatAlreadySatisfyScheduledHours()
  {
    Payroll payroll = new()
    {
      Date = new DateOnly(2025, 2, 10)
    };

    payroll.UpdatePayrollByAttendance(
      type: "Regular",
      basicPay: 26000m,
      dailyRate: 1000m,
      hourlyRate: 125m,
      hdmfCon: 100m,
      hdmfEr: 100m,
      totalWorkHours: 80m,
      totalLateHrs: 0m,
      totalUnderHrs: 0m,
      totalOverbreakHrs: 0m,
      totalNightShiftHrs: 0m,
      totalLeaves: 2m,
      restDayHrs: 0m,
      regularHolidayHrs: 0m,
      specialHolidayHrs: 0m,
      overtimeHrs: 0m,
      nightOtHrs: 0m,
      nightRegHolHrs: 0m,
      nightSpecHolHrs: 0m,
      otRestDayHrs: 0m,
      otRegHolHrs: 0m,
      otSpHolHrs: 0m,
      cola: 0m,
      monal: 0m,
      salun: 0m,
      refabs: 0m,
      refut: 0m,
      refot: 0m,
      absences: 0,
      totalMonthlyAmortization: 0m,
      hasScheduledWork: true,
      scheduledDays: 10);

    payroll.LaborHrs.Should().Be(80m);
    payroll.Leaves.Should().Be(2m);
    payroll.LeavesAmount.Should().Be(0m);
    payroll.GrossPayAmount.Should().Be(payroll.LaborHoursIncome);
  }

  [Fact]
  public void UpdatePayrollByAttendance_SetsBasicPayToZeroWhenPayrollPeriodHasNoScheduledWork()
  {
    Payroll payroll = new()
    {
      Date = new DateOnly(2025, 2, 10)
    };

    payroll.UpdatePayrollByAttendance(
      type: "Regular",
      basicPay: 26000m,
      dailyRate: 1000m,
      hourlyRate: 125m,
      hdmfCon: 100m,
      hdmfEr: 100m,
      totalWorkHours: 0m,
      totalLateHrs: 0m,
      totalUnderHrs: 0m,
      totalOverbreakHrs: 0m,
      totalNightShiftHrs: 0m,
      totalLeaves: 0m,
      restDayHrs: 0m,
      regularHolidayHrs: 0m,
      specialHolidayHrs: 0m,
      overtimeHrs: 0m,
      nightOtHrs: 0m,
      nightRegHolHrs: 0m,
      nightSpecHolHrs: 0m,
      otRestDayHrs: 0m,
      otRegHolHrs: 0m,
      otSpHolHrs: 0m,
      cola: 0m,
      monal: 0m,
      salun: 0m,
      refabs: 0m,
      refut: 0m,
      refot: 0m,
      absences: 0,
      totalMonthlyAmortization: 0m,
      hasScheduledWork: false,
      scheduledDays: 0);

    payroll.BasicPayAmount.Should().Be(0m);
    payroll.LaborHoursIncome.Should().Be(0m);
    payroll.GrossPayAmount.Should().Be(0m);
    payroll.LaborHrs.Should().Be(0m);
  }

  [Fact]
  public void UpdatePayrollByAttendance_UsesDailyRateTimesScheduledDaysForProbationaryEmployees()
  {
    Payroll payroll = new()
    {
      Date = new DateOnly(2025, 2, 10)
    };

    payroll.UpdatePayrollByAttendance(
      type: "Probationary",
      basicPay: 26000m,
      dailyRate: 1000m,
      hourlyRate: 125m,
      hdmfCon: 100m,
      hdmfEr: 100m,
      totalWorkHours: 80m,
      totalLateHrs: 0m,
      totalUnderHrs: 0m,
      totalOverbreakHrs: 0m,
      totalNightShiftHrs: 0m,
      totalLeaves: 0m,
      restDayHrs: 0m,
      regularHolidayHrs: 0m,
      specialHolidayHrs: 0m,
      overtimeHrs: 0m,
      nightOtHrs: 0m,
      nightRegHolHrs: 0m,
      nightSpecHolHrs: 0m,
      otRestDayHrs: 0m,
      otRegHolHrs: 0m,
      otSpHolHrs: 0m,
      cola: 0m,
      monal: 0m,
      salun: 0m,
      refabs: 0m,
      refut: 0m,
      refot: 0m,
      absences: 0,
      totalMonthlyAmortization: 0m,
      hasScheduledWork: true,
      scheduledDays: 11);

    payroll.BasicPayAmount.Should().Be(11000m);
    payroll.LaborHoursIncome.Should().Be(11000m);
    payroll.GrossPayAmount.Should().Be(11000m);
    payroll.UnionDues.Should().Be(0m);
  }

}
