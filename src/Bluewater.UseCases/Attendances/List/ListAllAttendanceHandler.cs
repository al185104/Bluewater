using System.Diagnostics;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Attendances.List;

internal class ListAllAttendanceHandler(IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAllAttendancesQuery, Result<IEnumerable<AllAttendancesDTO>>>
{
  public async Task<Result<IEnumerable<AllAttendancesDTO>>> Handle(ListAllAttendancesQuery request, CancellationToken cancellationToken)
  {
        // first get all employees
        List<EmployeeDTO> employees = new();
        using (var scope = serviceScopeFactory.CreateScope())
        {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new ListEmployeeQuery(null, null));
        if (ret.IsSuccess)
            employees = ret.Value.ToList();
        }

        if(employees.Count == 0) return Result<IEnumerable<AllAttendancesDTO>>.NotFound();

        List<AllAttendancesDTO> results = new();
        // get all Attendances per employee and per date. If no Attendance, create a default Attendance using ListAttendanceQuery
        foreach(var employee in employees) {
            using(var scope = serviceScopeFactory.CreateScope()) {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var ret = await mediator.Send(new ListAttendanceQuery(null, null, employee.Id, request.startDate, request.endDate));
                if(ret.IsSuccess) {
                var val = ret.Value.ToList();
        
                var (totalWorkHours, totalLateHours, totalUnderHours, totalLocked) = ProcessAttendance(val);
                results.Add(new AllAttendancesDTO(employee.Id, $"{employee.LastName}, {employee.FirstName}", employee.Department, employee.Section, employee.Charging, val, totalWorkHours, totalLateHours, totalUnderHours, totalLocked));
                }
            }
        }
        return Result<IEnumerable<AllAttendancesDTO>>.Success(results);
  }

  private (decimal totalWorkHours, decimal totalLateHours, decimal totalUnderHours, decimal totalLocked) ProcessAttendance(List<AttendanceDTO> val)
  {
    return (0, 0, 0, 0);
  }

}
