using Bluewater.App.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
  protected BaseViewModel(IActivityTraceService activityTraceService)
  {
    ActivityTraceService = activityTraceService;
  }

  protected IActivityTraceService ActivityTraceService { get; }

  [ObservableProperty]
  public partial bool IsBusy { get; set; }

  protected Task TraceCommandAsync(string name, object? args = null)
  {
    return ActivityTraceService.LogCommandAsync(name, args);
  }

  public virtual Task InitializeAsync() => Task.CompletedTask;
}
