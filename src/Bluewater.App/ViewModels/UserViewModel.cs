using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class UserViewModel : BaseViewModel
{
  public UserViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
