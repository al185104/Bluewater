using System.Xml;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using Bluewater.UseCases.Shifts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace Bluewater.UseCases.Timesheets.List;

internal class ListAllTimesheetHandler(IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAllTimesheetQuery, Result<IEnumerable<AllEmployeeTimesheetDTO>>>
{
  public async Task<Result<IEnumerable<AllEmployeeTimesheetDTO>>> Handle(ListAllTimesheetQuery request, CancellationToken cancellationToken)
  {
        List<EmployeeDTO> employees = new();
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var ret = await mediator.Send(new ListEmployeeQuery(null, null));
            if (ret.IsSuccess)
                employees = ret.Value.ToList();
        }

        List<AllEmployeeTimesheetDTO> results = new();
        // get all Timesheets per employee and per date. If no Timesheet, create a default Timesheet using ListTimesheetQuery
        foreach (var emp in employees)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var ret = await mediator.Send(new ListTimesheetQuery(null, null, emp.Id, request.startDate, request.endDate));
                if (ret.IsSuccess){
                    var val = ret.Value;

                    var totalWorkHours = 0;
                    var totalBreak = 0;
                    var totalLates = 0;
                    var TotalAbsents = 0;

                    results.Add(new AllEmployeeTimesheetDTO(val.EmployeeId, val.Name, val.Department, val.Section, val.Charging, val.Timesheets, totalWorkHours, totalBreak, totalLates, TotalAbsents));
                }
            }
        }
    return Result<IEnumerable<AllEmployeeTimesheetDTO>>.Success(results);
  }
}
