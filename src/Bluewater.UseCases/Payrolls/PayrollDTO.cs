namespace Bluewater.UseCases.Payrolls;
public record PayrollDTO()
{
        public Guid Id { get; init; }
        public Guid? EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? BankAccount { get; set; }
        public DateOnly Date { get; set; }  = DateOnly.FromDateTime(DateTime.Now); // done
        public string? Division { get; set; }
        public string? Department { get; set; }
        public string? Section { get; set; }
        public string? Position { get; set; }
        public string? Charging { get; set; }        
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


        // constructor
        public PayrollDTO(Guid id, Guid? employeeId, string? name, string barcode, string bankAccount, DateOnly date, decimal grossPayAmount, decimal netAmount, decimal basicPayAmount, decimal sssAmount, decimal sssERAmount, decimal pagibigAmount, decimal pagibigERAmount, decimal philhealthAmount, decimal philhealthERAmount, decimal restDayAmount, decimal restDayHrs, decimal regularHolidayAmount, decimal regularHolidayHrs, decimal specialHolidayAmount, decimal specialHolidayHrs, decimal overtimeAmount, decimal overtimeHrs, decimal nightDiffAmount, decimal nightDiffHrs, decimal nightDiffOvertimeAmount, decimal nightDiffOvertimeHrs, decimal nightDiffRegularHolidayAmount, decimal nightDiffRegularHolidayHrs, decimal nightDiffSpecialHolidayAmount, decimal nightDiffSpecialHolidayHrs, decimal overtimeRestDayAmount, decimal overtimeRestDayHrs, decimal overtimeRegularHolidayAmount, decimal overtimeRegularHolidayHrs, decimal overtimeSpecialHolidayAmount, decimal overtimeSpecialHolidayHrs, decimal unionDues, int absences, decimal absencesAmount, decimal leaves, decimal leavesAmount, decimal lates, decimal latesAmount, decimal undertime, decimal undertimeAmount, decimal overbreak, decimal overbreakAmount, decimal svcCharge, decimal costOfLivingAllowanceAmount, decimal monthlyAllowanceAmount, decimal salaryUnderpaymentAmount, decimal refundAbsencesAmount, decimal refundUndertimeAmount, decimal refundOvertimeAmount, decimal laborHoursIncome, decimal laborHrs, decimal taxDeductions, decimal totalConstantDeductions, decimal totalLoanDeductions, decimal totalDeductions) : this()
        {
            Id = id;
            EmployeeId = employeeId;
            Name = name;
            Barcode = barcode;
            BankAccount = bankAccount;
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
        }
}