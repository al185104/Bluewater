using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
  public LoginViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
  }

  [RelayCommand]
  async Task LoginAsync()
  {
    try
    {
      IsBusy = true;
      await TraceCommandAsync("Login", new { Target = nameof(HomePage) }).ConfigureAwait(false);
      MainThread.BeginInvokeOnMainThread(async () => { 
        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
      });
    }
    finally
    {
      IsBusy = false;
    }
  }
}
