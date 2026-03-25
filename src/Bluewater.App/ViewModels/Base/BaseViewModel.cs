using Bluewater.App.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    protected BaseViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    {
        ActivityTraceService = activityTraceService;
        ExceptionHandlingService = exceptionHandlingService;
    }

    protected readonly IActivityTraceService ActivityTraceService;

    protected readonly IExceptionHandlingService ExceptionHandlingService;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    protected Task TraceCommandAsync(string name, object? args = null)
    {
      return ActivityTraceService.LogCommandAsync(name, args);
    }

    partial void OnIsBusyChanged(bool value)
    {
      IsBusyChanged(value);
    }

    public virtual void IsBusyChanged(bool isBusy)
    {
      return;
    }

    protected Task NavigateAsync(string route)
    {
      return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(route));
    }

    protected Task NavigateAsync(string route, IDictionary<string, object> parameters)
    {
      return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(route, parameters));
    }

		[RelayCommand]
    public virtual Task InitializeAsync() => Task.CompletedTask;
}
