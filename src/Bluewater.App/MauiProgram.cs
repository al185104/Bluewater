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
    builder.Services.AddSingleton<LoginPage>();

    // viewmodels
    builder.Services.AddSingleton<LoginViewModel>();

    return builder.Build();
  }
}
