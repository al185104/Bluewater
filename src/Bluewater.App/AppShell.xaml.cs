using Bluewater.App.Interfaces;

namespace Bluewater.App;

public partial class AppShell : Shell
{
  private readonly IActivityTraceService activityTraceService;

  public AppShell(IActivityTraceService activityTraceService)
  {
    InitializeComponent();
    this.activityTraceService = activityTraceService;

    //Navigated += OnShellNavigated;
    //Navigating += OnShellNavigating;
  }

  //private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
  //{
  //  await activityTraceService
  //    .LogNavigationAsync(
  //      e.Previous?.Location.OriginalString,
  //      e.Current?.Location.OriginalString,
  //      new
  //      {
  //        Phase = "Navigated"
  //      })
  //    .ConfigureAwait(false);
  //}

  //private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
  //{
  //  await activityTraceService
  //    .LogNavigationAsync(
  //      e.Current?.Location.OriginalString,
  //      e.Target?.Location.OriginalString,
  //      new
  //      {
  //        Phase = "Navigating",
  //        e.Source
  //      })
  //    .ConfigureAwait(false);
  //}
}
