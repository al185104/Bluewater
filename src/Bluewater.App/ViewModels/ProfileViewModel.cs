using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
  public ProfileViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
