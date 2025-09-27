using Bluewater.App.ViewModels;
using Bluewater.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Bluewater.App;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
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

    // pages
    builder.Services.AddSingleton<AttendancePage>();
    builder.Services.AddSingleton<EmployeePage>();
    builder.Services.AddSingleton<HomePage>();
    builder.Services.AddSingleton<LeavePage>();
    builder.Services.AddSingleton<LoginPage>();
    builder.Services.AddSingleton<MealCreditPage>();
    builder.Services.AddSingleton<PayrollPage>();
    builder.Services.AddSingleton<ProfilePage>();
    builder.Services.AddSingleton<SchedulePage>();
    builder.Services.AddSingleton<SettingPage>();
    builder.Services.AddSingleton<ShiftsPage>();
    builder.Services.AddSingleton<TimesheetPage>();
    builder.Services.AddSingleton<UserPage>();

    // viewmodels
    builder.Services.AddSingleton<AttendanceViewModel>();
    builder.Services.AddSingleton<EmployeeViewModel>();
    builder.Services.AddSingleton<HomeViewModel>();
    builder.Services.AddSingleton<LeaveViewModel>();
    builder.Services.AddSingleton<LoginViewModel>();
    builder.Services.AddSingleton<MealCreditViewModel>();
    builder.Services.AddSingleton<PayrollViewModel>();
    builder.Services.AddSingleton<ProfileViewModel>();
    builder.Services.AddSingleton<ScheduleViewModel>();
    builder.Services.AddSingleton<SettingViewModel>();
    builder.Services.AddSingleton<ShiftsViewModel>();
    builder.Services.AddSingleton<TimesheetViewModel>();
    builder.Services.AddSingleton<UserViewModel>();

    return builder.Build();
  }
}
