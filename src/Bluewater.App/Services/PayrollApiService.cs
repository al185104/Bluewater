using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class PayrollApiService(IApiClient apiClient) : IPayrollApiService
{
  public async Task<IReadOnlyList<PayrollSummary>> GetPayrollsAsync(
    DateOnly startDate,
    DateOnly endDate,
    string? chargingName = null,
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildListRequestUri(startDate, endDate, chargingName, tenant, skip, take);

    PayrollListResponseDto? response = await apiClient.GetAsync<PayrollListResponseDto>(requestUri, cancellationToken);

    if (response?.Payrolls is not { Count: > 0 })
    {
      return Array.Empty<PayrollSummary>();
    }

    return response.Payrolls
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<IReadOnlyList<PayrollGroupedSummary>> GetGroupedPayrollsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildGroupedRequestUri(skip, take);

    PayrollGroupedListResponseDto? response = await apiClient.GetAsync<PayrollGroupedListResponseDto>(requestUri, cancellationToken);

    if (response?.Payrolls is not { Count: > 0 })
    {
      return Array.Empty<PayrollGroupedSummary>();
    }

    return response.Payrolls
      .Where(dto => dto is not null)
      .Select(MapToGroupedSummary)
      .ToList();
  }

  public async Task<PayrollSummary?> GetPayrollByIdAsync(Guid payrollId, CancellationToken cancellationToken = default)
  {
    if (payrollId == Guid.Empty)
    {
      throw new ArgumentException("Payroll ID must be provided", nameof(payrollId));
    }

    PayrollDto? dto = await apiClient.GetAsync<PayrollDto>($"Payrolls/{payrollId}", cancellationToken);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<Guid?> CreatePayrollAsync(PayrollSummary payroll, CancellationToken cancellationToken = default)
  {
    if (payroll is null)
    {
      throw new ArgumentNullException(nameof(payroll));
    }

    if (!payroll.EmployeeId.HasValue || payroll.EmployeeId.Value == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(payroll));
    }

    CreatePayrollRequestDto request = BuildCreateRequest(payroll);

    CreatePayrollResponseDto? response = await apiClient.PostAsync<CreatePayrollRequestDto, CreatePayrollResponseDto>(
      CreatePayrollRequestDto.Route,
      request,
      cancellationToken);

    return response?.PayrollId;
  }

  public async Task<PayrollSummary?> UpdatePayrollAsync(PayrollSummary payroll, CancellationToken cancellationToken = default)
  {
    if (payroll is null)
    {
      throw new ArgumentNullException(nameof(payroll));
    }

    if (payroll.Id == Guid.Empty)
    {
      throw new ArgumentException("Payroll ID must be provided", nameof(payroll));
    }

    if (!payroll.EmployeeId.HasValue || payroll.EmployeeId.Value == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(payroll));
    }

    UpdatePayrollRequestDto request = BuildUpdateRequest(payroll);

    UpdatePayrollResponseDto? response = await apiClient.PutAsync<UpdatePayrollRequestDto, UpdatePayrollResponseDto>(
      UpdatePayrollRequestDto.BuildRoute(payroll.Id),
      request,
      cancellationToken);

    return response?.Payroll is null ? null : MapToSummary(response.Payroll);
  }

  public Task<bool> DeletePayrollAsync(Guid payrollId, CancellationToken cancellationToken = default)
  {
    if (payrollId == Guid.Empty)
    {
      throw new ArgumentException("Payroll ID must be provided", nameof(payrollId));
    }

    return apiClient.DeleteAsync($"Payrolls/{payrollId}", cancellationToken);
  }

  private static string BuildListRequestUri(
    DateOnly startDate,
    DateOnly endDate,
    string? chargingName,
    TenantDto tenant,
    int? skip,
    int? take)
  {
    List<string> parameters =
    [
      $"startDate={startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"endDate={endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}",
      $"tenant={tenant}"
    ];

    if (!string.IsNullOrWhiteSpace(chargingName))
    {
      parameters.Add($"chargingName={Uri.EscapeDataString(chargingName)}");
    }

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    string query = string.Join('&', parameters);
    return $"Payrolls?{query}";
  }

  private static string BuildGroupedRequestUri(int? skip, int? take)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    if (parameters.Count == 0)
    {
      return "Payrolls/Grouped";
    }

    string query = string.Join('&', parameters);
    return $"Payrolls/Grouped?{query}";
  }

  private static CreatePayrollRequestDto BuildCreateRequest(PayrollSummary payroll)
  {
    return new CreatePayrollRequestDto
    {
      EmployeeId = payroll.EmployeeId!.Value,
      Date = payroll.Date,
      GrossPayAmount = payroll.GrossPayAmount,
      NetAmount = payroll.NetAmount,
      BasicPayAmount = payroll.BasicPayAmount,
      SssAmount = payroll.SssAmount,
      SssERAmount = payroll.SssERAmount,
      PagibigAmount = payroll.PagibigAmount,
      PagibigERAmount = payroll.PagibigERAmount,
      PhilhealthAmount = payroll.PhilhealthAmount,
      PhilhealthERAmount = payroll.PhilhealthERAmount,
      RestDayAmount = payroll.RestDayAmount,
      RestDayHrs = payroll.RestDayHrs,
      RegularHolidayAmount = payroll.RegularHolidayAmount,
      RegularHolidayHrs = payroll.RegularHolidayHrs,
      SpecialHolidayAmount = payroll.SpecialHolidayAmount,
      SpecialHolidayHrs = payroll.SpecialHolidayHrs,
      OvertimeAmount = payroll.OvertimeAmount,
      OvertimeHrs = payroll.OvertimeHrs,
      NightDiffAmount = payroll.NightDiffAmount,
      NightDiffHrs = payroll.NightDiffHrs,
      NightDiffOvertimeAmount = payroll.NightDiffOvertimeAmount,
      NightDiffOvertimeHrs = payroll.NightDiffOvertimeHrs,
      NightDiffRegularHolidayAmount = payroll.NightDiffRegularHolidayAmount,
      NightDiffRegularHolidayHrs = payroll.NightDiffRegularHolidayHrs,
      NightDiffSpecialHolidayAmount = payroll.NightDiffSpecialHolidayAmount,
      NightDiffSpecialHolidayHrs = payroll.NightDiffSpecialHolidayHrs,
      OvertimeRestDayAmount = payroll.OvertimeRestDayAmount,
      OvertimeRestDayHrs = payroll.OvertimeRestDayHrs,
      OvertimeRegularHolidayAmount = payroll.OvertimeRegularHolidayAmount,
      OvertimeRegularHolidayHrs = payroll.OvertimeRegularHolidayHrs,
      OvertimeSpecialHolidayAmount = payroll.OvertimeSpecialHolidayAmount,
      OvertimeSpecialHolidayHrs = payroll.OvertimeSpecialHolidayHrs,
      UnionDues = payroll.UnionDues,
      Absences = payroll.Absences,
      AbsencesAmount = payroll.AbsencesAmount,
      Leaves = payroll.Leaves,
      LeavesAmount = payroll.LeavesAmount,
      Lates = payroll.Lates,
      LatesAmount = payroll.LatesAmount,
      Undertime = payroll.Undertime,
      UndertimeAmount = payroll.UndertimeAmount,
      Overbreak = payroll.Overbreak,
      OverbreakAmount = payroll.OverbreakAmount,
      SvcCharge = payroll.SvcCharge,
      CostOfLivingAllowanceAmount = payroll.CostOfLivingAllowanceAmount,
      MonthlyAllowanceAmount = payroll.MonthlyAllowanceAmount,
      SalaryUnderpaymentAmount = payroll.SalaryUnderpaymentAmount,
      RefundAbsencesAmount = payroll.RefundAbsencesAmount,
      RefundUndertimeAmount = payroll.RefundUndertimeAmount,
      RefundOvertimeAmount = payroll.RefundOvertimeAmount,
      LaborHoursIncome = payroll.LaborHoursIncome,
      LaborHrs = payroll.LaborHrs,
      TaxDeductions = payroll.TaxDeductions,
      TotalConstantDeductions = payroll.TotalConstantDeductions,
      TotalLoanDeductions = payroll.TotalLoanDeductions,
      TotalDeductions = payroll.TotalDeductions
    };
  }

  private static UpdatePayrollRequestDto BuildUpdateRequest(PayrollSummary payroll)
  {
    return new UpdatePayrollRequestDto
    {
      PayrollId = payroll.Id,
      Id = payroll.Id,
      EmployeeId = payroll.EmployeeId!.Value,
      Date = payroll.Date,
      GrossPayAmount = payroll.GrossPayAmount,
      NetAmount = payroll.NetAmount,
      BasicPayAmount = payroll.BasicPayAmount,
      SssAmount = payroll.SssAmount,
      SssERAmount = payroll.SssERAmount,
      PagibigAmount = payroll.PagibigAmount,
      PagibigERAmount = payroll.PagibigERAmount,
      PhilhealthAmount = payroll.PhilhealthAmount,
      PhilhealthERAmount = payroll.PhilhealthERAmount,
      RestDayAmount = payroll.RestDayAmount,
      RestDayHrs = payroll.RestDayHrs,
      RegularHolidayAmount = payroll.RegularHolidayAmount,
      RegularHolidayHrs = payroll.RegularHolidayHrs,
      SpecialHolidayAmount = payroll.SpecialHolidayAmount,
      SpecialHolidayHrs = payroll.SpecialHolidayHrs,
      OvertimeAmount = payroll.OvertimeAmount,
      OvertimeHrs = payroll.OvertimeHrs,
      NightDiffAmount = payroll.NightDiffAmount,
      NightDiffHrs = payroll.NightDiffHrs,
      NightDiffOvertimeAmount = payroll.NightDiffOvertimeAmount,
      NightDiffOvertimeHrs = payroll.NightDiffOvertimeHrs,
      NightDiffRegularHolidayAmount = payroll.NightDiffRegularHolidayAmount,
      NightDiffRegularHolidayHrs = payroll.NightDiffRegularHolidayHrs,
      NightDiffSpecialHolidayAmount = payroll.NightDiffSpecialHolidayAmount,
      NightDiffSpecialHolidayHrs = payroll.NightDiffSpecialHolidayHrs,
      OvertimeRestDayAmount = payroll.OvertimeRestDayAmount,
      OvertimeRestDayHrs = payroll.OvertimeRestDayHrs,
      OvertimeRegularHolidayAmount = payroll.OvertimeRegularHolidayAmount,
      OvertimeRegularHolidayHrs = payroll.OvertimeRegularHolidayHrs,
      OvertimeSpecialHolidayAmount = payroll.OvertimeSpecialHolidayAmount,
      OvertimeSpecialHolidayHrs = payroll.OvertimeSpecialHolidayHrs,
      UnionDues = payroll.UnionDues,
      Absences = payroll.Absences,
      AbsencesAmount = payroll.AbsencesAmount,
      Leaves = payroll.Leaves,
      LeavesAmount = payroll.LeavesAmount,
      Lates = payroll.Lates,
      LatesAmount = payroll.LatesAmount,
      Undertime = payroll.Undertime,
      UndertimeAmount = payroll.UndertimeAmount,
      Overbreak = payroll.Overbreak,
      OverbreakAmount = payroll.OverbreakAmount,
      SvcCharge = payroll.SvcCharge,
      CostOfLivingAllowanceAmount = payroll.CostOfLivingAllowanceAmount,
      MonthlyAllowanceAmount = payroll.MonthlyAllowanceAmount,
      SalaryUnderpaymentAmount = payroll.SalaryUnderpaymentAmount,
      RefundAbsencesAmount = payroll.RefundAbsencesAmount,
      RefundUndertimeAmount = payroll.RefundUndertimeAmount,
      RefundOvertimeAmount = payroll.RefundOvertimeAmount,
      LaborHoursIncome = payroll.LaborHoursIncome,
      LaborHrs = payroll.LaborHrs,
      TaxDeductions = payroll.TaxDeductions,
      TotalConstantDeductions = payroll.TotalConstantDeductions,
      TotalLoanDeductions = payroll.TotalLoanDeductions,
      TotalDeductions = payroll.TotalDeductions
    };
  }

  private static PayrollSummary MapToSummary(PayrollDto dto)
  {
    return new PayrollSummary
    {
      Id = dto.Id,
      EmployeeId = dto.EmployeeId,
      Name = dto.Name,
      Barcode = dto.Barcode,
      BankAccount = dto.BankAccount,
      Date = dto.Date,
      Division = dto.Division,
      Department = dto.Department,
      Section = dto.Section,
      Position = dto.Position,
      Charging = dto.Charging,
      GrossPayAmount = dto.GrossPayAmount,
      NetAmount = dto.NetAmount,
      BasicPayAmount = dto.BasicPayAmount,
      SssAmount = dto.SssAmount,
      SssERAmount = dto.SssERAmount,
      PagibigAmount = dto.PagibigAmount,
      PagibigERAmount = dto.PagibigERAmount,
      PhilhealthAmount = dto.PhilhealthAmount,
      PhilhealthERAmount = dto.PhilhealthERAmount,
      RestDayAmount = dto.RestDayAmount,
      RestDayHrs = dto.RestDayHrs,
      RegularHolidayAmount = dto.RegularHolidayAmount,
      RegularHolidayHrs = dto.RegularHolidayHrs,
      SpecialHolidayAmount = dto.SpecialHolidayAmount,
      SpecialHolidayHrs = dto.SpecialHolidayHrs,
      OvertimeAmount = dto.OvertimeAmount,
      OvertimeHrs = dto.OvertimeHrs,
      NightDiffAmount = dto.NightDiffAmount,
      NightDiffHrs = dto.NightDiffHrs,
      NightDiffOvertimeAmount = dto.NightDiffOvertimeAmount,
      NightDiffOvertimeHrs = dto.NightDiffOvertimeHrs,
      NightDiffRegularHolidayAmount = dto.NightDiffRegularHolidayAmount,
      NightDiffRegularHolidayHrs = dto.NightDiffRegularHolidayHrs,
      NightDiffSpecialHolidayAmount = dto.NightDiffSpecialHolidayAmount,
      NightDiffSpecialHolidayHrs = dto.NightDiffSpecialHolidayHrs,
      OvertimeRestDayAmount = dto.OvertimeRestDayAmount,
      OvertimeRestDayHrs = dto.OvertimeRestDayHrs,
      OvertimeRegularHolidayAmount = dto.OvertimeRegularHolidayAmount,
      OvertimeRegularHolidayHrs = dto.OvertimeRegularHolidayHrs,
      OvertimeSpecialHolidayAmount = dto.OvertimeSpecialHolidayAmount,
      OvertimeSpecialHolidayHrs = dto.OvertimeSpecialHolidayHrs,
      UnionDues = dto.UnionDues,
      Absences = dto.Absences,
      AbsencesAmount = dto.AbsencesAmount,
      Leaves = dto.Leaves,
      LeavesAmount = dto.LeavesAmount,
      Lates = dto.Lates,
      LatesAmount = dto.LatesAmount,
      Undertime = dto.Undertime,
      UndertimeAmount = dto.UndertimeAmount,
      Overbreak = dto.Overbreak,
      OverbreakAmount = dto.OverbreakAmount,
      SvcCharge = dto.SvcCharge,
      CostOfLivingAllowanceAmount = dto.CostOfLivingAllowanceAmount,
      MonthlyAllowanceAmount = dto.MonthlyAllowanceAmount,
      SalaryUnderpaymentAmount = dto.SalaryUnderpaymentAmount,
      RefundAbsencesAmount = dto.RefundAbsencesAmount,
      RefundUndertimeAmount = dto.RefundUndertimeAmount,
      RefundOvertimeAmount = dto.RefundOvertimeAmount,
      LaborHoursIncome = dto.LaborHoursIncome,
      LaborHrs = dto.LaborHrs,
      TaxDeductions = dto.TaxDeductions,
      TotalConstantDeductions = dto.TotalConstantDeductions,
      TotalLoanDeductions = dto.TotalLoanDeductions,
      TotalDeductions = dto.TotalDeductions
    };
  }

  private static PayrollGroupedSummary MapToGroupedSummary(PayrollSummaryDto dto)
  {
    return new PayrollGroupedSummary
    {
      Date = dto.Date,
      Count = dto.Count,
      TotalServiceCharge = dto.TotalServiceCharge,
      TotalAbsences = dto.TotalAbsences,
      TotalLeaves = dto.TotalLeaves,
      TotalLates = dto.TotalLates,
      TotalUndertimes = dto.TotalUndertimes,
      TotalOverbreak = dto.TotalOverbreak,
      TotalTaxDeductions = dto.TotalTaxDeductions,
      TotalNetAmount = dto.TotalNetAmount
    };
  }
}
