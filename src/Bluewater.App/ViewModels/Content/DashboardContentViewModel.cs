using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels.Content;

public partial class DashboardContentViewModel : BaseViewModel
{
  public DashboardContentViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService) : base(activityTraceService, exceptionHandlingService)
  {
  }
}
