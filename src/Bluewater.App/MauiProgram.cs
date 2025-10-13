using System;
using System.Net.Http;
using Bluewater.App.Interfaces;
using Bluewater.App.Services;
using Bluewater.App.ViewModels;
using Bluewater.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bluewater.App;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    const string ApiBaseAddress = "https://localhost:57679";

    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .UseMauiCommunityToolkit()
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
    builder.Services.AddSingleton<ILeaveApiService, LeaveApiService>();
    builder.Services.AddSingleton<IAttendanceApiService, AttendanceApiService>();
    builder.Services.AddSingleton<IPayrollApiService, PayrollApiService>();
    builder.Services.AddSingleton<IScheduleApiService, ScheduleApiService>();
    builder.Services.AddSingleton<IActivityTraceService, ActivityTraceService>();
    builder.Services.AddSingleton<IExceptionHandlingService, ExceptionHandlingService>();

    builder.Services.AddSingleton<AppShell>();

    // pages
    builder.Services.AddTransient<AttendancePage>();
    builder.Services.AddTransient<EmployeePage>();
    builder.Services.AddTransient<HomePage>();
    builder.Services.AddTransient<LeavePage>();
    builder.Services.AddTransient<LoginPage>();
    builder.Services.AddTransient<MealCreditPage>();
    builder.Services.AddTransient<PayrollPage>();
    builder.Services.AddTransient<ProfilePage>();
    builder.Services.AddTransient<SchedulePage>();
    builder.Services.AddTransient<SettingPage>();
    builder.Services.AddTransient<ShiftsPage>();
    builder.Services.AddTransient<TimesheetPage>();
    builder.Services.AddTransient<UserPage>();

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
    builder.Services.AddTransient<TimesheetViewModel>();
    builder.Services.AddTransient<UserViewModel>();

    MauiApp app = builder.Build();

    IExceptionHandlingService exceptionHandlingService = app.Services.GetRequiredService<IExceptionHandlingService>();
    exceptionHandlingService.Initialize();

    return app;
  }
}
