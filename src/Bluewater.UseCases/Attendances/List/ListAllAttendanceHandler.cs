using Ardalis.Result;
using Ardalis.SharedKernel;
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
                var val = ret.Value.OrderByDescending(i => i.EntryDate).ToList();
        
                var (totalWorkHours, totalLateHours, totalUnderHours, totalLeaves) = ProcessAttendance(val);
                results.Add(new AllAttendancesDTO(employee.Id, $"{employee.LastName}, {employee.FirstName}", employee.Department, employee.Section, employee.Charging, val, totalWorkHours, totalLateHours, totalUnderHours, totalLeaves));
              }
            }
        }
        return Result<IEnumerable<AllAttendancesDTO>>.Success(results);
  }

  private (decimal totalWorkHours, decimal totalLateHours, decimal totalUnderHours, decimal totalLeaves) ProcessAttendance(List<AttendanceDTO> val)
  {
    //sum of all work hours
    var _totalWorkHours = val.Sum(i => i.WorkHrs);
    var _totalLateHours = val.Sum(i => i.LateHrs);
    var _totalUnderHours = val.Sum(i => i.UnderHrs);
    var _totalLeaves = val.Count(i => i.LeaveId != null && i.LeaveId != Guid.Empty);

    return (_totalLateHours ?? 0, _totalLateHours ?? 0, _totalUnderHours ?? 0, _totalLeaves);
  }

}
