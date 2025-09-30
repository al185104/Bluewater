using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class SettingViewModel : BaseViewModel
{
  public SettingViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
