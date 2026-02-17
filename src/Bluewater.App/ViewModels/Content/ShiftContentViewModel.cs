using System.Collections.ObjectModel;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels.Content;

public partial class ShiftContentViewModel : BaseViewModel
{
		private CancellationTokenSource? _initCts;
		private readonly IShiftApiService _shiftApiService;
		private readonly IActivityTraceService _activityTraceService;
		private readonly IExceptionHandlingService _exceptionHandlingService;

		public ObservableCollection<ShiftSummary>? Shifts { get; set; } = new();

		public ShiftContentViewModel(
				IShiftApiService shiftApiService,
				IActivityTraceService activityTraceService, 
				IExceptionHandlingService exceptionHandlingService) : base(activityTraceService, exceptionHandlingService)
		{
				_shiftApiService = shiftApiService;
				_activityTraceService = activityTraceService;
				_exceptionHandlingService = exceptionHandlingService;
		}

		public override async Task InitializeAsync()
		{
				_initCts?.Cancel();
				_initCts?.Dispose();

				_initCts = new CancellationTokenSource();
				CancellationToken ct = _initCts.Token;

				try
				{
						IsBusy = true;
						var shifts = await _shiftApiService.GetShiftsAsync(ct);
						if (shifts != null && shifts.Count > 0) 
						{
								Shifts!.Clear();
								foreach (var shift in shifts) 
								{
										Shifts.Add(shift);
								}
						}
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("Shifts initialization was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Initializing shifts failed.");
				}
				finally
				{
						IsBusy = false;
				}
		}

		public void Dispose()
		{
				if (_initCts != null)
				{
						_initCts.Cancel();
						_initCts.Dispose();
						_initCts = null;
				}

				Shifts!.Clear();
				Shifts = null;
		}
}
