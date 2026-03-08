using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels.Modals;

public partial class PayrollDetailsViewModel : BaseViewModel, IQueryAttributable
{
		public PayrollDetailsViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService) : base(activityTraceService, exceptionHandlingService)
		{
		}

		public void ApplyQueryAttributes(IDictionary<string, object> query)
		{
				throw new NotImplementedException();
		}
}
