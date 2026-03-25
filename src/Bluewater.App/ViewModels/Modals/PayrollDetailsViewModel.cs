using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.ViewModels.Modals;

public partial class PayrollDetailsViewModel : BaseViewModel, IQueryAttributable
{
		[ObservableProperty]
		public partial PayrollSummary? Payroll { get; set; }

		public string EmployeeName => Payroll?.Name ?? "N/A";
		public string OrganizationDisplay => $"{Payroll?.Division ?? "-"} • {Payroll?.Department ?? "-"} • {Payroll?.Section ?? "-"}";
		public string PositionDisplay => Payroll?.Position ?? "-";
		public string ChargingDisplay => Payroll?.Charging ?? "-";
		public string PayrollDateDisplay => Payroll is null ? "-" : Payroll.Date.ToString("MMMM dd, yyyy");

		public decimal TotalPremiumsAmount =>
				(Payroll?.RestDayAmount ?? 0m) +
				(Payroll?.RegularHolidayAmount ?? 0m) +
				(Payroll?.SpecialHolidayAmount ?? 0m) +
				(Payroll?.OvertimeAmount ?? 0m) +
				(Payroll?.NightDiffAmount ?? 0m) +
				(Payroll?.NightDiffOvertimeAmount ?? 0m) +
				(Payroll?.NightDiffRegularHolidayAmount ?? 0m) +
				(Payroll?.NightDiffSpecialHolidayAmount ?? 0m) +
				(Payroll?.OvertimeRestDayAmount ?? 0m) +
				(Payroll?.OvertimeRegularHolidayAmount ?? 0m) +
				(Payroll?.OvertimeSpecialHolidayAmount ?? 0m);

		public decimal TotalAllowancesAmount =>
				(Payroll?.SvcCharge ?? 0m) +
				(Payroll?.CostOfLivingAllowanceAmount ?? 0m) +
				(Payroll?.MonthlyAllowanceAmount ?? 0m) +
				(Payroll?.SalaryUnderpaymentAmount ?? 0m) +
				(Payroll?.LaborHoursIncome ?? 0m) +
				(Payroll?.RefundAbsencesAmount ?? 0m) +
				(Payroll?.RefundUndertimeAmount ?? 0m) +
				(Payroll?.RefundOvertimeAmount ?? 0m);

		public decimal TotalGovernmentDeductionsAmount =>
				(Payroll?.SssAmount ?? 0m) +
				(Payroll?.PagibigAmount ?? 0m) +
				(Payroll?.PhilhealthAmount ?? 0m);

		public decimal TotalTimeBasedDeductionsAmount =>
				(Payroll?.AbsencesAmount ?? 0m) +
				(Payroll?.LeavesAmount ?? 0m) +
				(Payroll?.LatesAmount ?? 0m) +
				(Payroll?.UndertimeAmount ?? 0m) +
				(Payroll?.OverbreakAmount ?? 0m);

		public PayrollDetailsViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
				: base(activityTraceService, exceptionHandlingService)
		{
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				if (query.TryGetValue("Payroll", out object? value) && value is PayrollSummary payroll)
				{
						Payroll = payroll;
						_ = TraceCommandAsync(nameof(ApplyQueryAttributes), new { PayrollId = payroll.Id, payroll.EmployeeId, payroll.Date });
						RaiseComputedProperties();
				}
		}

		partial void OnPayrollChanged(PayrollSummary? value)
		{
				RaiseComputedProperties();
		}

		private void RaiseComputedProperties()
		{
				OnPropertyChanged(nameof(EmployeeName));
				OnPropertyChanged(nameof(OrganizationDisplay));
				OnPropertyChanged(nameof(PositionDisplay));
				OnPropertyChanged(nameof(ChargingDisplay));
				OnPropertyChanged(nameof(PayrollDateDisplay));
				OnPropertyChanged(nameof(TotalPremiumsAmount));
				OnPropertyChanged(nameof(TotalAllowancesAmount));
				OnPropertyChanged(nameof(TotalGovernmentDeductionsAmount));
				OnPropertyChanged(nameof(TotalTimeBasedDeductionsAmount));
		}
}
