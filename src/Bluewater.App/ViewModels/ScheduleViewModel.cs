using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
  public ScheduleViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
  }
}
