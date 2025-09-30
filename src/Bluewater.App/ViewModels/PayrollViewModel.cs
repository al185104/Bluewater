using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels;

public partial class PayrollViewModel : BaseViewModel
{
  public PayrollViewModel(IActivityTraceService activityTraceService)
    : base(activityTraceService)
  {
  }
}
