using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Attendances;
using Bluewater.UseCases.Attendances.List;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Timesheets.List;

internal class ListAllTimesheetHandler(
  IRepository<AppUser> _userRepository,
  IRepository<Payroll> payrollRepository,
  IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAllTimesheetQuery, Result<Common.PagedResult<AllEmployeeTimesheetDTO>>>
{
  public async Task<Result<Common.PagedResult<AllEmployeeTimesheetDTO>>> Handle(ListAllTimesheetQuery request, CancellationToken cancellationToken)
  {
    List<EmployeeDTO> employees = new();
    int totalCount = 0;

    using (var scope = serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      if (request.employeeId.HasValue)
      {
        Result<EmployeeDTO> employeeResult = await mediator.Send(new GetEmployeeQuery(request.employeeId.Value), cancellationToken);
        if (employeeResult.IsSuccess)
        {
          employees = [employeeResult.Value];
          totalCount = employees.Count;
        }
      }
      else
      {
        Result<Common.PagedResult<EmployeeDTO>> employeeResult = string.IsNullOrWhiteSpace(request.charging)
          ? await mediator.Send(new ListEmployeeQuery(request.skip, request.take, request.tenant), cancellationToken)
          : await mediator.Send(new ListEmployeeByChargingQuery(request.skip, request.take, request.charging, request.tenant), cancellationToken);

        if (employeeResult.IsSuccess)
        {
          employees = employeeResult.Value.Items.ToList();
          totalCount = employeeResult.Value.TotalCount;
        }
      }
    }

    if (employees.Count == 0)
    {
      return Result<Common.PagedResult<AllEmployeeTimesheetDTO>>.NotFound();
    }

    List<AllEmployeeTimesheetDTO> results = new();
    IReadOnlyCollection<Guid> employeeIds = employees.Select(employee => employee.Id).Distinct().ToList();
    HashSet<Guid> payrollCreatedEmployeeIds = [];

    if (employeeIds.Count > 0)
    {
      List<Payroll> existingPayrolls = await payrollRepository.ListAsync(
        new PayrollByEmployeeIdsAndDateSpec(employeeIds, request.endDate),
        cancellationToken);

      payrollCreatedEmployeeIds = existingPayrolls
        .Where(payroll => payroll.EmployeeId.HasValue)
        .Select(payroll => payroll.EmployeeId!.Value)
        .ToHashSet();
    }

    foreach (EmployeeDTO employee in employees)
    {
      var user = await _userRepository.GetByIdAsync(employee.User?.Id ?? Guid.Empty, cancellationToken);
      if (user is null)
      {
        continue;
      }

      using (var scope = serviceScopeFactory.CreateScope())
      {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        Result<EmployeeTimesheetDTO> timesheetResult = await mediator.Send(
          new ListTimesheetQuery(null, null, user.Username, request.startDate, request.endDate),
          cancellationToken);

        if (timesheetResult.IsSuccess)
        {
          EmployeeTimesheetDTO val = timesheetResult.Value;

          decimal totalWorkHours = 0;
          decimal totalBreak = 0;
          decimal totalLates = 0;
          int totalAbsents = 0;
          decimal totalUndertimes = 0;
          decimal totalOverbreaks = 0;
          decimal totalLeaves = 0;

          Result<IEnumerable<AttendanceDTO>> attendanceResult = await mediator.Send(
            new ListAttendanceQuery(null, null, employee.Id, request.startDate, request.endDate),
            cancellationToken);

          if (attendanceResult.IsSuccess)
          {
            (totalWorkHours, totalBreak, totalLates, totalAbsents, totalUndertimes, totalOverbreaks, totalLeaves) = ProcessAttendanceSummary(attendanceResult.Value.ToList());
          }

          results.Add(new AllEmployeeTimesheetDTO(
            val.EmployeeId,
            val.Name,
            val.Department,
            val.Section,
            val.Charging,
            val.Timesheets,
            totalWorkHours,
            totalBreak,
            totalLates,
            totalAbsents,
            totalUndertimes,
            totalOverbreaks,
            totalLeaves,
            payrollCreatedEmployeeIds.Contains(employee.Id)));
        }
      }
    }

    return Result<Common.PagedResult<AllEmployeeTimesheetDTO>>.Success(new Common.PagedResult<AllEmployeeTimesheetDTO>(results, totalCount));
  }

  private static (decimal totalWorkHours, decimal totalBreak, decimal totalLates, int totalAbsents, decimal totalUndertimes, decimal totalOverbreaks, decimal totalLeaves) ProcessAttendanceSummary(List<AttendanceDTO> attendances)
  {
    var totalWorkHours = attendances.Sum(i => i.WorkHrs) ?? 0;
    var totalBreak = attendances.Sum(i => i.OverbreakHrs) ?? 0;
    var totalLates = attendances.Sum(i => i.LateHrs) ?? 0;
    var totalAbsents = AttendanceSummaryCalculator.CountAbsencesExcludingApprovedLeaves(attendances);

    decimal totalUndertimes = attendances.Sum(i => i.UnderHrs) ?? 0;
    decimal totalOverbreaks = attendances.Sum(i => i.OverbreakHrs) ?? 0;
    decimal totalLeaves = AttendanceSummaryCalculator.CountApprovedLeaves(attendances);

    return (totalWorkHours, totalBreak, totalLates, totalAbsents, totalUndertimes, totalOverbreaks, totalLeaves);
  }
}
