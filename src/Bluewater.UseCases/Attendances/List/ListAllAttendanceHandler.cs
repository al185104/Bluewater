using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Attendances.List;

internal class ListAllAttendanceHandler(IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAllAttendancesQuery, Result<PagedResult<AllAttendancesDTO>>>
{
  public async Task<Result<PagedResult<AllAttendancesDTO>>> Handle(ListAllAttendancesQuery request, CancellationToken cancellationToken)
  {
    try{
      // first get all employees
      List<EmployeeDTO> employees = new();
      int totalCount = 0;
      using (var scope = serviceScopeFactory.CreateScope())
      {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        //var ret = await mediator.Send(new ListEmployeeQuery(null, null));
        var ret = await mediator.Send(new ListEmployeeByChargingQuery(request.skip, request.take, request.charging, request.tenant));
        if (ret.IsSuccess)
        {
            employees = ret.Value.Items.ToList();
            totalCount = ret.Value.TotalCount;
        }
            //employees = ret.Value.Where(i => !string.IsNullOrEmpty(i.Charging) && i.Charging.Equals(request.charging, StringComparison.InvariantCultureIgnoreCase)).ToList();
      }

      if(employees.Count == 0) return Result<PagedResult<AllAttendancesDTO>>.NotFound();

      List<AllAttendancesDTO> results = new();
      // get all Attendances per employee and per date. If no Attendance, create a default Attendance using ListAttendanceQuery
      foreach(var employee in employees) {
          using(var scope = serviceScopeFactory.CreateScope()) {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var ret = await mediator.Send(new ListAttendanceQuery(null, null, employee.Id, request.startDate, request.endDate));
            if(ret.IsSuccess) {
              var val = ret.Value.ToList();
      
              var (totalWorkHours, totalAbsences, totalLateHours, totalUnderHours, totalOverbreakHrs, totalNightShiftHrs, totalLeaves) = ProcessAttendance(val);
              
              results.Add(new AllAttendancesDTO(employee.Id, employee.User?.Username ?? string.Empty, $"{employee.LastName}, {employee.FirstName}", employee.Department, employee.Section, employee.Charging, val, totalWorkHours, totalAbsences, totalLateHours, totalUnderHours, totalOverbreakHrs, totalNightShiftHrs, totalLeaves));
            }
          }
      }
      return Result<PagedResult<AllAttendancesDTO>>.Success(new PagedResult<AllAttendancesDTO>(results, totalCount));
    }
    catch(Exception){
      throw;
    }
  }

  private (decimal totalWorkHours, int totalAbsences, decimal totalLateHours, decimal totalUnderHours, decimal totalOverbreakHrs, decimal TotalNightShiftHours, decimal totalLeaves) ProcessAttendance(List<AttendanceDTO> val)
  {
    //sum of all work hours
    var _totalWorkHours = val.Sum(i => i.WorkHrs);
    var _totalAbsences = val.Count(i => i.ShiftId != null && i.Shift != null && !i.Shift.Name.Equals("R", StringComparison.InvariantCultureIgnoreCase)) - val.Count(i => i.WorkHrs > 0);
    var _totalLateHours = val.Sum(i => i.LateHrs);
    var _totalUnderHours = val.Sum(i => i.UnderHrs);
    var _totalOverbreakHrs = val.Sum(i => i.OverbreakHrs);
    var _totalNightShiftHours = val.Sum(i => i.NightShiftHours);

    var _totalLeaves = val.Count(i => i.LeaveId != null && i.LeaveId != Guid.Empty);

    return (_totalWorkHours ?? 0, _totalAbsences, _totalLateHours ?? 0, _totalUnderHours ?? 0, _totalOverbreakHrs ?? 0, _totalNightShiftHours ?? 0, _totalLeaves);
  }

}
