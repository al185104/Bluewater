using Microsoft.Extensions.DependencyInjection;
#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
#endif

namespace Bluewater.App;

public partial class App : Application
{
  private readonly IServiceProvider serviceProvider;

  public App(IServiceProvider serviceProvider)
  {
    InitializeComponent();
    this.serviceProvider = serviceProvider;
  }

  public IServiceProvider Services => serviceProvider;

  protected override Window CreateWindow(IActivationState? activationState)
  {
    var window = new Window(serviceProvider.GetRequiredService<AppShell>());

#if WINDOWS
    window.Created += (sender, args) =>
    {
      if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
      {
        var appWindow = nativeWindow.GetAppWindow();
        appWindow?.SetPresenter(AppWindowPresenterKind.FullScreen);
      }
    };
#endif

    return window;
  }
}
