using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class LeaveViewModel : BaseViewModel
{
  public LeaveViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
  }
}
