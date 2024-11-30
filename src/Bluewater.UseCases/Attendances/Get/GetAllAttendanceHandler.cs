using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.Attendances.Get;
using Bluewater.UseCases.Employees;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Attendances.List;

internal class GetAllAttendanceHandler(IServiceScopeFactory serviceScopeFactory) : IQueryHandler<GetAllAttendanceQuery, Result<AllAttendancesDTO>>
{
  public async Task<Result<AllAttendancesDTO>> Handle(GetAllAttendanceQuery request, CancellationToken cancellationToken)
  {
        EmployeeShortDTO employee = default!;
        using (var scope = serviceScopeFactory.CreateScope())
        {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var ret = await mediator.Send(new GetEmployeeShortQuery(request.name));
          if (ret.IsSuccess)
              employee = ret.Value;
        }

        if(employee == null) return Result<AllAttendancesDTO>.NotFound();

        AllAttendancesDTO result = default!;
        using (var scope = serviceScopeFactory.CreateScope())
        {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var ret = await mediator.Send(new ListAttendanceQuery(null, null, employee.Id, request.startDate, request.endDate));
          if (ret.IsSuccess)
          {
            var val = ret.Value.ToList();
            var (totalWorkHours, totalAbsences, totalLateHours, totalUnderHours, totalOverbreakHrs, totalNightShiftHrs, totalLocked) = ProcessAttendance(val);
            result = new AllAttendancesDTO(employee.Id, employee.Barcode, employee.Name, employee.Department, employee.Section, employee.Charging, val, totalWorkHours, totalAbsences, totalLateHours, totalUnderHours, totalOverbreakHrs, totalNightShiftHrs, totalLocked);
          }
        }

        return Result<AllAttendancesDTO>.Success(result);
  }

  private (decimal totalWorkHours, int totalAbsences, decimal totalLateHours, decimal totalUnderHours, decimal totalOverbreakHrs, decimal totalNightShiftHrs, decimal totalLocked) ProcessAttendance(List<AttendanceDTO> val)
  {
    return (0, 0, 0, 0, 0, 0, 0);
  }

}
