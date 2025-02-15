using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.Core.TimesheetAggregate.Specifications;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.Get;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Timesheets.List;

internal class ListTimesheetHandler(IRepository<Timesheet> _repository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListTimesheetQuery, Result<EmployeeTimesheetDTO>>
{
  public async Task<Result<EmployeeTimesheetDTO>> Handle(ListTimesheetQuery request, CancellationToken cancellationToken)
  {
    //get employee first by spec
    EmployeeShortDTO? emp = null;
    using (var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new GetEmployeeShortQuery(request.name));
        if (ret.IsSuccess)
            emp = ret.Value;
    }

    if(emp == null) {
      using (var scope = serviceScopeFactory.CreateScope()) {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new GetEmployeeByBarcodeQuery(request.name));
        if (ret.IsSuccess)
          emp = ret.Value;
      }
    }

    if (emp == null) return Result<EmployeeTimesheetDTO>.NotFound();

    // get all the timesheet from specified start and end dates from request
    var spec = new TimesheetByEmpIdAndStartEndDateSpec(emp!.Id, request.startDate, request.endDate);
    var timesheets = await _repository.ListAsync(spec, cancellationToken);

    // loop for dates and create default Timesheet if not exists
    List<TimesheetInfo> results = new();
    for (var date = request.startDate; date <= request.endDate; date = date.AddDays(1))
    {
        var timesheet = timesheets!.FirstOrDefault(s => s.EntryDate == date);
        if(timesheet == null){
          results.Add(new TimesheetInfo(Guid.Empty, null, null, null, null, date, isEdited: false));
          // results.Add(new TimesheetInfo(Guid.Empty, new DateTime(date.Year, date.Month, date.Day), new DateTime(date.Year, date.Month, date.Day), new DateTime(date.Year, date.Month, date.Day), new DateTime(date.Year, date.Month, date.Day), date, isEdited: false));
        }
        else
          results.Add(new TimesheetInfo(timesheet.Id, timesheet.TimeIn1, timesheet.TimeOut1, timesheet.TimeIn2, timesheet.TimeOut2, timesheet.EntryDate, isEdited: timesheet.IsEdited));
    }

    return Result<EmployeeTimesheetDTO>.Success(new EmployeeTimesheetDTO(emp!.Id, emp.Name, emp.Department ?? string.Empty, emp.Section ?? string.Empty, emp.Charging ?? string.Empty, results.OrderByDescending(i => i.EntryDate).ToList()));
  }
}
