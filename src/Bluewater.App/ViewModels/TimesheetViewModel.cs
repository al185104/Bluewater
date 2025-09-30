using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class TimesheetViewModel : BaseViewModel
{
  public TimesheetViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
