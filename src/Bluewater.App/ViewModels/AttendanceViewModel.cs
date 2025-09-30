using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class AttendanceViewModel : BaseViewModel
{
  public AttendanceViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
