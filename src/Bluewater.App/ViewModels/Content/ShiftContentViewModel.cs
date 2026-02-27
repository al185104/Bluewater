using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

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

		[RelayCommand]
		public async Task ImportShiftsAsync()
		{
				_initCts?.Cancel();
				_initCts?.Dispose();

				_initCts = new CancellationTokenSource();
				CancellationToken ct = _initCts.Token;

				try
				{
						IsBusy = true;

						var customFileType = new FilePickerFileType(
								new Dictionary<DevicePlatform, IEnumerable<string>>
								{
										{ DevicePlatform.WinUI, new[] { ".csv" } },
										{ DevicePlatform.macOS, new[] { "csv" } },
								});

						PickOptions options = new()
						{
								PickerTitle = "Please select a shift import file",
								FileTypes = customFileType,
						};

						var result = await FilePicker.PickAsync(options);

						if (result is null)
						{
								await Shell.Current.DisplayAlert("Import warning", "There is nothing to import. Please select a correct shift import file.", "Okay");
								return;
						}

						var shiftsToImport = new List<ShiftSummary>();

						using var stream = await result.OpenReadAsync();
						using var reader = new StreamReader(stream, Encoding.UTF8);

						var headerLine = await reader.ReadLineAsync();
						if (string.IsNullOrWhiteSpace(headerLine))
						{
								await Shell.Current.DisplayAlert("Import error", "Unable to read shift import headers.", "Back");
								return;
						}

						var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();

						while (!reader.EndOfStream)
						{
								var line = await reader.ReadLineAsync();
								if (string.IsNullOrWhiteSpace(line))
								{
										continue;
								}

								var values = line.Split(',');

								string Get(string headerName)
								{
										var index = headers.FindIndex(h => h.Equals(headerName, StringComparison.OrdinalIgnoreCase));
										return index >= 0 && index < values.Length ? values[index].Trim() : string.Empty;
								}

								var name = Get("Name");
								if (string.IsNullOrWhiteSpace(name))
								{
										continue;
								}

								shiftsToImport.Add(new ShiftSummary
								{
										Id = Guid.NewGuid(),
										Name = name,
										ShiftStartTime = NormalizeTime(Get("ShiftStartTime")),
										ShiftBreakTime = NormalizeTime(Get("ShiftBreakTime")),
										ShiftBreakEndTime = NormalizeTime(Get("ShiftBreakEndTime")),
										ShiftEndTime = NormalizeTime(Get("ShiftEndTime")),
										BreakHours = ParseDecimal(Get("BreakHours"))
								});
						}

						int successfulImports = 0;
						foreach (var shift in shiftsToImport)
						{
								var created = await _shiftApiService.CreateShiftAsync(shift, ct);
								if (created is null)
								{
										continue;
								}

								successfulImports++;
						}

						await Shell.Current.DisplayAlert("Import complete", $"Successfully imported {successfulImports} shift(s).", "Okay");
						await InitializeAsync();
				}
				catch (OperationCanceledException)
				{
						await _activityTraceService.LogCommandAsync("Shift import was canceled.");
				}
				catch (Exception ex)
				{
						_exceptionHandlingService.Handle(ex, "Importing shifts failed.");
				}
				finally
				{
						IsBusy = false;
				}
		}

		[RelayCommand]
		public Task ExportShiftsAsync()
		{
				return Task.CompletedTask;
		}

		private static decimal ParseDecimal(string value)
		{
				return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
						? parsed
						: 0m;
		}

		private static string NormalizeTime(string value)
		{
				if (string.IsNullOrWhiteSpace(value))
				{
						return string.Empty;
				}

				if (TimeOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
				{
						return parsedTime.ToString("HH:mm", CultureInfo.InvariantCulture);
				}

				if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var parsedSpan))
				{
						return parsedSpan.ToString("hh\\:mm", CultureInfo.InvariantCulture);
				}

				return value;
		}
}
