using Bluewater.App.Interfaces;
using Bluewater.App.Services;
using Bluewater.App.ViewModels;
using Bluewater.App.ViewModels.Content;
using Bluewater.App.ViewModels.Modals;
using Bluewater.App.Views;
using Bluewater.App.Views.Controls;
using Bluewater.App.Views.Modals;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;
using Windows.Devices.Usb;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Bluewater.App;

public static class MauiProgram
{
		public static MauiApp CreateMauiApp()
		{
				const string ApiBaseAddress = "https://localhost:57679";

				var builder = MauiApp.CreateBuilder();
				builder
					.UseMauiApp<App>()
					.UseMauiCommunityToolkit(options => options.SetShouldEnableSnackbarOnWindows(true))
					.ConfigureLifecycleEvents(lifecycle =>
					{
#if WINDOWS
							lifecycle.AddWindows(windows =>
							{
									windows.OnWindowCreated(window =>
									{
											var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
											var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
											var appWindow = AppWindow.GetFromWindowId(windowId);

											appWindow.SetPresenter(AppWindowPresenterKind.Default);
									});
							});
#endif
					})
					.ConfigureFonts(fonts =>
					{
							fonts.AddFont("Feather.ttf", "Icons");
							fonts.AddFont("Nunito-Light.ttf", "NunitoLight");
							fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
							fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
							fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
							fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
							fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
							fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					});

#if DEBUG
				builder.Logging.AddDebug();
#endif

				// services
				builder.Services.AddSingleton(sp =>
				{
#if DEBUG
						var handler = new HttpClientHandler
						{
								ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
						};
#else
      var handler = new HttpClientHandler();
#endif

						return new HttpClient(handler)
						{
								BaseAddress = new Uri(ApiBaseAddress)
						};
				});

				builder.Services.AddSingleton<IApiClient, ApiClient>();
				builder.Services.AddSingleton<IEmployeeApiService, EmployeeApiService>();
				builder.Services.AddSingleton<IShiftApiService, ShiftApiService>();
				builder.Services.AddSingleton<IDepartmentApiService, DepartmentApiService>();
				builder.Services.AddSingleton<IDivisionApiService, DivisionApiService>();
				builder.Services.AddSingleton<IPositionApiService, PositionApiService>();
				builder.Services.AddSingleton<ISectionApiService, SectionApiService>();
				builder.Services.AddSingleton<IChargingApiService, ChargingApiService>();
				builder.Services.AddSingleton<IHolidayApiService, HolidayApiService>();
				builder.Services.AddSingleton<IEmployeeTypeApiService, EmployeeTypeApiService>();
				builder.Services.AddSingleton<ILevelApiService, LevelApiService>();
				builder.Services.AddSingleton<ILeaveCreditApiService, LeaveCreditApiService>();
				builder.Services.AddSingleton<ILeaveApiService, LeaveApiService>();
				builder.Services.AddSingleton<IAttendanceApiService, AttendanceApiService>();
				builder.Services.AddSingleton<ITimesheetApiService, TimesheetApiService>();
				builder.Services.AddSingleton<IPayrollApiService, PayrollApiService>();
				builder.Services.AddSingleton<IPayApiService, PayApiService>();
				builder.Services.AddSingleton<IUserApiService, UserApiService>();
				builder.Services.AddSingleton<IScheduleApiService, ScheduleApiService>();
				builder.Services.AddSingleton<IDashboardApiService, DashboardApiService>();
				builder.Services.AddSingleton<IActivityTraceService, ActivityTraceService>();
				builder.Services.AddSingleton<IExceptionHandlingService, ExceptionHandlingService>();
				builder.Services.AddSingleton<IReferenceDataService, ReferenceDataService>();

				builder.Services.AddSingleton<AppShell>();

				// viewmodels
				builder.Services.AddTransient<AttendanceViewModel>();
				builder.Services.AddTransient<EmployeeViewModel>();
				builder.Services.AddTransient<HomeViewModel>();
				builder.Services.AddTransient<LeaveViewModel>();
				builder.Services.AddTransient<LoginViewModel>();
				builder.Services.AddTransient<MealCreditViewModel>();
				builder.Services.AddTransient<PayrollViewModel>();
				builder.Services.AddTransient<ProfileViewModel>();
				builder.Services.AddTransient<ScheduleViewModel>();
				builder.Services.AddTransient<SettingViewModel>();
				builder.Services.AddTransient<ShiftsViewModel>();
				builder.Services.AddTransient<TimesheetsViewModel>();
				builder.Services.AddTransient<UserViewModel>();

				// controls
				builder.Services.AddTransient<DashboardView>();
				builder.Services.AddTransient<EmployeesView>();
				builder.Services.AddTransient<ShiftsView>();
				builder.Services.AddTransient<SchedulesView>();
				builder.Services.AddTransient<MealCreditView>();
				builder.Services.AddTransient<LeavesView>();
				builder.Services.AddTransient<TimesheetView>();
				builder.Services.AddTransient<PayrollView>();
				builder.Services.AddTransient<AttendanceView>();
				builder.Services.AddTransient<UsersView>();
				builder.Services.AddTransient<SettingsView>();
				// modals
				builder.Services.AddTransient<EmployeeDetailsPage>();
				builder.Services.AddTransient<TimesheetDetailsPage>();


				// control viewmodels
				builder.Services.AddTransient<DashboardContentViewModel>();
				builder.Services.AddTransient<EmployeeContentViewModel>();
				builder.Services.AddTransient<ShiftContentViewModel>();
				builder.Services.AddTransient<ScheduleContentViewModel>();
				// modals
				builder.Services.AddTransient<EmployeeDetailsViewModel>();
				builder.Services.AddTransient<TimesheetDetailsViewModel>();

				MauiApp app = builder.Build();

				IExceptionHandlingService exceptionHandlingService = app.Services.GetRequiredService<IExceptionHandlingService>();
				exceptionHandlingService.Initialize();

				IReferenceDataService referenceDataService = app.Services.GetRequiredService<IReferenceDataService>();
				try
				{
						referenceDataService.InitializeAsync().GetAwaiter().GetResult();
				}
				catch (Exception ex)
				{
						exceptionHandlingService.Handle(ex, "Initializing reference data");
				}

				return app;
		}
}
