using Bluewater.App.ViewModels.Base;
using Bluewater.App.Views;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
  [RelayCommand]
  async Task LoginAsync()
  {
    try
    {
      IsBusy = true;
      await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
    }
    finally
    {
      IsBusy = false;
    }
  }
}
