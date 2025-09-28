using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.ViewModels.Base;

public partial class BaseViewModel : ObservableObject
{
  [ObservableProperty]
  public partial bool IsBusy { get; set; }

  public virtual Task InitializeAsync() => Task.CompletedTask;
}
