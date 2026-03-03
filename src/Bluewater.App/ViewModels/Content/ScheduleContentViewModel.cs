using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;

namespace Bluewater.App.ViewModels.Content;

public partial class ScheduleContentViewModel : BaseViewModel
{


		public ScheduleContentViewModel(
				IActivityTraceService activityTraceService,
				IExceptionHandlingService exceptionHandlingService,
				IScheduleApiService scheduleApiService,
				IReferenceDataService referenceDataService) : base(activityTraceService, exceptionHandlingService)
		{
		}

		public override async Task InitializeAsync()
		{

		}
}
