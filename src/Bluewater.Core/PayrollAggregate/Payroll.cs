using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.PayrollAggregate.Helpers;

namespace Bluewater.Core.PayrollAggregate;
public class Payroll : EntityBase<Guid>, IAggregateRoot
{
    public DateOnly Date { get; set; }  = DateOnly.FromDateTime(DateTime.Now); // done
    public decimal GrossPayAmount { get; set; } // done
    public decimal NetAmount { get; set; } // done
    public decimal BasicPayAmount { get; set; } // done
    public decimal SSSAmount { get; set; } // done
    public decimal SSSERAmount { get; set; } // done
    public decimal PagibigAmount { get; set; } // done
    public decimal PagibigERAmount { get; set; } // done
    public decimal PhilhealthAmount { get; set; } // done
    public decimal PhilhealthERAmount { get; set; } // done

    //Work on Rest Day premium
    public decimal RestDayAmount { get; set; } // done
    public decimal RestDayHrs { get; set; } // done

    //Work on Regular Holiday premium
    public decimal RegularHolidayAmount { get; set; } // done
    public decimal RegularHolidayHrs { get; set; } // done

    //Work on Special Holiday premium
    public decimal SpecialHolidayAmount { get; set; } // done
    public decimal SpecialHolidayHrs { get; set; } // done

    //Overtime
    public decimal OvertimeAmount { get; set; } // done
    public decimal OvertimeHrs { get; set; } // done

    //Night Shift Differential
    public decimal NightDiffAmount { get; set; } // done
    public decimal NightDiffHrs { get; set; } // done

    //Night Shift Differential on Overtime
    public decimal NightDiffOvertimeAmount { get; set; } // done
    public decimal NightDiffOvertimeHrs { get; set; } // done

    //Night Shift Differential on Regular Holiday
    public decimal NightDiffRegularHolidayAmount { get; set; } // done
    public decimal NightDiffRegularHolidayHrs { get; set; } // done

    //Night Shift Differential on Special Holiday
    public decimal NightDiffSpecialHolidayAmount { get; set; } // done
    public decimal NightDiffSpecialHolidayHrs { get; set; } // done

    //Overtime on Rest Day premium
    public decimal OvertimeRestDayAmount { get; set; } // done
    public decimal OvertimeRestDayHrs { get; set; } // done

    //Overtime on Regular Holiday premium
    public decimal OvertimeRegularHolidayAmount { get; set; } // done
    public decimal OvertimeRegularHolidayHrs { get; set; } // done

    //Overtime on Special Holiday premium
    public decimal OvertimeSpecialHolidayAmount { get; set; } // done
    public decimal OvertimeSpecialHolidayHrs { get; set; } // done

    public decimal UnionDues { get; set;} // done
    public int Absences { get; set; } // done
    public decimal AbsencesAmount { get; set; } // done
    public decimal Leaves { get; set; } // done
    public decimal LeavesAmount { get; set; } // done
    public decimal Lates { get; set; } // done
    public decimal LatesAmount { get; set; } // done  
    public decimal Undertime { get; set; } // done
    public decimal UndertimeAmount { get; set; } // done
    public decimal Overbreak { get; set; } // done
    public decimal OverbreakAmount { get; set; } // done
    public decimal SvcCharge { get; set; } // done
    //Cost of Living Allowance
    public decimal CostOfLivingAllowanceAmount { get; set; } // done
    //Monthly Allowance
    public decimal MonthlyAllowanceAmount { get; set; } // done
    //Salary Underpayment
    public decimal SalaryUnderpaymentAmount { get; set; } // done
    //Refund Absences
    public decimal RefundAbsencesAmount { get; set; } // done
    //Refund Undertime
    public decimal RefundUndertimeAmount { get; set; } // done
    //Refund Overtime
    public decimal RefundOvertimeAmount { get; set; } // done

    public decimal LaborHoursIncome { get; set; } // done
    public decimal LaborHrs { get; set; } // done
    public decimal TaxDeductions { get; set; } // done
    public decimal TaxPercentage { get; set; } // not added
    public decimal TotalConstantDeductions { get; set; } // done
    public decimal TotalLoanDeductions { get; set; } // done
    public decimal TotalDeductions { get; set; } // done
    
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;        

    public Guid? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // create constructor with complete parameters
    public Payroll()
    {
        EmployeeId = null;
        Date = DateOnly.FromDateTime(DateTime.Now);
        GrossPayAmount = 0;
        NetAmount = 0;
        BasicPayAmount = 0;
        SSSAmount = 0;
        SSSERAmount = 0;
        PagibigAmount = 0;
        PagibigERAmount = 0;
        PhilhealthAmount = 0;
        PhilhealthERAmount = 0;
        RestDayAmount = 0;
        RestDayHrs = 0;
        RegularHolidayAmount = 0;
        RegularHolidayHrs = 0;
        SpecialHolidayAmount = 0;
        SpecialHolidayHrs = 0;
        OvertimeAmount = 0;
        OvertimeHrs = 0;
        NightDiffAmount = 0;
        NightDiffHrs = 0;
        NightDiffOvertimeAmount = 0;
        NightDiffOvertimeHrs = 0;
        NightDiffRegularHolidayAmount = 0;
        NightDiffRegularHolidayHrs = 0;
        NightDiffSpecialHolidayAmount = 0;
        NightDiffSpecialHolidayHrs = 0;
        OvertimeRestDayAmount = 0;
        OvertimeRestDayHrs = 0;
        OvertimeRegularHolidayAmount = 0;
        OvertimeRegularHolidayHrs = 0;
        OvertimeSpecialHolidayAmount = 0;
        OvertimeSpecialHolidayHrs = 0;
        UnionDues = 0;
        Absences = 0;
        AbsencesAmount = 0;
        Leaves = 0;
        LeavesAmount = 0;
        Lates = 0;
        LatesAmount = 0;
        Undertime = 0;
        UndertimeAmount = 0;
        Overbreak = 0;
        OverbreakAmount = 0;
        SvcCharge = 0;
        CostOfLivingAllowanceAmount = 0;
        MonthlyAllowanceAmount = 0;
        SalaryUnderpaymentAmount = 0;
        RefundAbsencesAmount = 0;
        RefundUndertimeAmount = 0;
        RefundOvertimeAmount = 0;
        LaborHoursIncome = 0;
        LaborHrs = 0;
        TaxDeductions = 0;
        TotalConstantDeductions = 0;
        TotalLoanDeductions = 0;
        TotalDeductions = 0;        
    }

    public Payroll(Guid employeeId, DateOnly date, decimal grossPayAmount, decimal netAmount, decimal basicPayAmount, decimal sssAmount, decimal sssERAmount, decimal pagibigAmount, decimal pagibigERAmount, decimal philhealthAmount, decimal philhealthERAmount, decimal restDayAmount, decimal restDayHrs, decimal regularHolidayAmount, decimal regularHolidayHrs, decimal specialHolidayAmount, decimal specialHolidayHrs, decimal overtimeAmount, decimal overtimeHrs, decimal nightDiffAmount, decimal nightDiffHrs, decimal nightDiffOvertimeAmount, decimal nightDiffOvertimeHrs, decimal nightDiffRegularHolidayAmount, decimal nightDiffRegularHolidayHrs, decimal nightDiffSpecialHolidayAmount, decimal nightDiffSpecialHolidayHrs, decimal overtimeRestDayAmount, decimal overtimeRestDayHrs, decimal overtimeRegularHolidayAmount, decimal overtimeRegularHolidayHrs, decimal overtimeSpecialHolidayAmount, decimal overtimeSpecialHolidayHrs, decimal unionDues, int absences, decimal absencesAmount, decimal leaves, decimal leavesAmount, decimal lates, decimal latesAmount, decimal undertime, decimal undertimeAmount, decimal overbreak, decimal overbreakAmount, decimal svcCharge, decimal costOfLivingAllowanceAmount, decimal monthlyAllowanceAmount, decimal salaryUnderpaymentAmount, decimal refundAbsencesAmount, decimal refundUndertimeAmount, decimal refundOvertimeAmount, decimal laborHoursIncome, decimal laborHrs, decimal taxDeductions, decimal totalConstantDeductions, decimal totalLoanDeductions, decimal totalDeductions)
    {
        Date = date;
        GrossPayAmount = grossPayAmount;
        NetAmount = netAmount;
        BasicPayAmount = basicPayAmount;
        SSSAmount = sssAmount;
        SSSERAmount = sssERAmount;
        PagibigAmount = pagibigAmount;
        PagibigERAmount = pagibigERAmount;
        PhilhealthAmount = philhealthAmount;
        PhilhealthERAmount = philhealthERAmount;
        RestDayAmount = restDayAmount;
        RestDayHrs = restDayHrs;
        RegularHolidayAmount = regularHolidayAmount;
        RegularHolidayHrs = regularHolidayHrs;
        SpecialHolidayAmount = specialHolidayAmount;
        SpecialHolidayHrs = specialHolidayHrs;
        OvertimeAmount = overtimeAmount;
        OvertimeHrs = overtimeHrs;
        NightDiffAmount = nightDiffAmount;
        NightDiffHrs = nightDiffHrs;
        NightDiffOvertimeAmount = nightDiffOvertimeAmount;
        NightDiffOvertimeHrs = nightDiffOvertimeHrs;
        NightDiffRegularHolidayAmount = nightDiffRegularHolidayAmount;
        NightDiffRegularHolidayHrs = nightDiffRegularHolidayHrs;
        NightDiffSpecialHolidayAmount = nightDiffSpecialHolidayAmount;
        NightDiffSpecialHolidayHrs = nightDiffSpecialHolidayHrs;
        OvertimeRestDayAmount = overtimeRestDayAmount;
        OvertimeRestDayHrs = overtimeRestDayHrs;
        OvertimeRegularHolidayAmount = overtimeRegularHolidayAmount;
        OvertimeRegularHolidayHrs = overtimeRegularHolidayHrs;
        OvertimeSpecialHolidayAmount = overtimeSpecialHolidayAmount;
        OvertimeSpecialHolidayHrs = overtimeSpecialHolidayHrs;
        UnionDues = unionDues;
        Absences = absences;
        AbsencesAmount = absencesAmount;
        Leaves = leaves;
        LeavesAmount = leavesAmount;
        Lates = lates;
        LatesAmount = latesAmount;
        Undertime = undertime;
        UndertimeAmount = undertimeAmount;
        Overbreak = overbreak;
        OverbreakAmount = overbreakAmount;
        SvcCharge = svcCharge;
        CostOfLivingAllowanceAmount = costOfLivingAllowanceAmount;
        MonthlyAllowanceAmount = monthlyAllowanceAmount;
        SalaryUnderpaymentAmount = salaryUnderpaymentAmount;
        RefundAbsencesAmount = refundAbsencesAmount;
        RefundUndertimeAmount = refundUndertimeAmount;
        RefundOvertimeAmount = refundOvertimeAmount;
        LaborHoursIncome = laborHoursIncome;
        LaborHrs = laborHrs;
        TaxDeductions = taxDeductions;
        TotalConstantDeductions = totalConstantDeductions;
        TotalLoanDeductions = totalLoanDeductions;
        TotalDeductions = totalDeductions;
        EmployeeId = employeeId;
    }

    // need to filter this. some parameters may be assigned directly.
    public void UpdatePayrollByAttendance(
        string type, 
        decimal basicPay, decimal dailyRate, decimal hourlyRate, decimal hdmfCon, decimal hdmfEr, 
        decimal totalWorkHours, decimal totalLateHrs, decimal totalUnderHrs, decimal totalOverbreakHrs, decimal totalNightShiftHrs, decimal totalLeaves,
        decimal restDayHrs, decimal regularHolidayHrs, decimal specialHolidayHrs, decimal overtimeHrs, decimal nightOtHrs, decimal nightRegHolHrs, decimal nightSpecHolHrs, decimal otRestDayHrs, decimal otRegHolHrs, decimal otSpHolHrs,
        decimal cola, decimal monal, decimal salun, decimal refabs, decimal refut, decimal refot,
        int absences, decimal totalMonthlyAmortization)
    {
        BasicPayAmount = basicPay;
        LaborHoursIncome = BasicPayAmount / 2; // semi-monthly
        LaborHrs = totalWorkHours;

        RestDayHrs = restDayHrs;
        RestDayAmount = Math.Round(restDayHrs * hourlyRate, 2);

        RegularHolidayHrs = regularHolidayHrs;
        RegularHolidayAmount = Math.Round(regularHolidayHrs * hourlyRate, 2);

        SpecialHolidayHrs = specialHolidayHrs;
        SpecialHolidayAmount = Math.Round(specialHolidayHrs * hourlyRate * 0.30m, 2);

        OvertimeHrs = overtimeHrs;
        OvertimeAmount = Math.Round(overtimeHrs * hourlyRate * 1.25m, 2);

        NightDiffHrs = totalNightShiftHrs;
        NightDiffAmount = Math.Round(totalNightShiftHrs * hourlyRate * 0.10m, 2);

        NightDiffOvertimeHrs = nightOtHrs;
        NightDiffOvertimeAmount = Math.Round(nightOtHrs * hourlyRate * 0.10m * 1.25m, 2);

        NightDiffRegularHolidayHrs = nightRegHolHrs;
        NightDiffRegularHolidayAmount = Math.Round(nightRegHolHrs * hourlyRate * 2.0m * 0.1m, 2);

        NightDiffSpecialHolidayHrs = nightSpecHolHrs;
        NightDiffSpecialHolidayAmount = Math.Round(nightSpecHolHrs * hourlyRate * 1.3m * 0.1m, 2);

        OvertimeRestDayHrs = otRestDayHrs;
        OvertimeRestDayAmount = Math.Round(otRestDayHrs * hourlyRate * 1.3m * 1.3m, 2);

        OvertimeRegularHolidayHrs = otRegHolHrs;
        OvertimeRegularHolidayAmount = Math.Round(otRegHolHrs * hourlyRate * 2.0m * 1.3m, 2);

        OvertimeSpecialHolidayHrs = otSpHolHrs;
        OvertimeSpecialHolidayAmount = Math.Round(otSpHolHrs * hourlyRate * 1.3m * 1.3m, 2);

        Undertime = totalUnderHrs;
        UndertimeAmount = Math.Round(totalUnderHrs * hourlyRate, 2);

        Absences = absences;
        AbsencesAmount = Math.Round(absences * dailyRate, 2);

        Lates = totalLateHrs;
        LatesAmount = Math.Round(totalLateHrs * hourlyRate, 2);

        Overbreak = totalOverbreakHrs;
        OverbreakAmount = Math.Round(totalOverbreakHrs * hourlyRate, 2);

        Leaves = totalLeaves;
        LeavesAmount = Math.Round(totalLeaves * dailyRate, 2);

        GrossPayAmount = LaborHoursIncome
        /*premiums*/ + RestDayAmount + RegularHolidayAmount + SpecialHolidayAmount + OvertimeAmount + NightDiffAmount + NightDiffOvertimeAmount + NightDiffRegularHolidayAmount + NightDiffSpecialHolidayAmount + OvertimeRestDayAmount + OvertimeRegularHolidayAmount + OvertimeSpecialHolidayAmount
        /*other earnings*/ + cola + monal + salun + refabs + refut + refot;

        // TODO leave without pay
        UnionDues = string.Equals(type, "Regular", StringComparison.OrdinalIgnoreCase) ? 50 : 0;
        var adjustedGrossIncome = GrossPayAmount - AbsencesAmount - LatesAmount - UndertimeAmount - OverbreakAmount - UnionDues;

        // + contant deductions
        PagibigAmount = Date.Day != 15 ? 0 : Math.Round(hdmfCon, 2);
        PagibigERAmount = Date.Day != 15 ? 0 : Math.Round(hdmfEr, 2);
        PhilhealthAmount = Math.Round(basicPay * 0.05m / 2, 2);
        PhilhealthERAmount = PhilhealthAmount;
        (decimal ER, decimal EE) = CompensationLookup.FindValuesByCompensation(basicPay);
        SSSAmount = Date.Day == 15 ? 0 : Math.Round(EE);
        SSSERAmount = Date.Day == 15 ? 0 : Math.Round(ER);
        // - constant deductions

        var (taxDeduction, taxPercentage) = TaxCalculator.CalculateTax(adjustedGrossIncome + SvcCharge - PagibigAmount - PhilhealthAmount - SSSAmount);
        TaxDeductions = Math.Round(taxDeduction, 2);
        TaxPercentage = Math.Round(taxPercentage, 2);

        TotalConstantDeductions = Math.Round(TaxDeductions + PagibigAmount + PhilhealthAmount + SSSAmount + UndertimeAmount + LatesAmount + OverbreakAmount + AbsencesAmount + LeavesAmount + UnionDues, 2);
        TotalLoanDeductions = Math.Round(totalMonthlyAmortization, 2);
        TotalDeductions = Math.Round(TotalLoanDeductions + TotalConstantDeductions, 2);

        NetAmount = Math.Round(GrossPayAmount - TotalDeductions, 2);
    }

}