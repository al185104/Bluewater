using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.UseCases.Common;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Employees.List;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Timesheets.List;

internal class ListAllTimesheetHandler(IRepository<AppUser> _userRepository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAllTimesheetQuery, Result<PagedResult<AllEmployeeTimesheetDTO>>>
{
  public async Task<Result<PagedResult<AllEmployeeTimesheetDTO>>> Handle(ListAllTimesheetQuery request, CancellationToken cancellationToken)
  {
    List<EmployeeDTO> employees = new();
    int totalCount = 0;

    using (var scope = serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      Result<PagedResult<EmployeeDTO>> employeeResult = string.IsNullOrWhiteSpace(request.charging)
        ? await mediator.Send(new ListEmployeeQuery(request.skip, request.take, request.tenant), cancellationToken)
        : await mediator.Send(new ListEmployeeByChargingQuery(request.skip, request.take, request.charging, request.tenant), cancellationToken);

      if (employeeResult.IsSuccess)
      {
        employees = employeeResult.Value.Items.ToList();
        totalCount = employeeResult.Value.TotalCount;
      }
    }

    if (employees.Count == 0)
    {
      return Result<PagedResult<AllEmployeeTimesheetDTO>>.NotFound();
    }

    List<AllEmployeeTimesheetDTO> results = new();

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

          var totalWorkHours = 0;
          var totalBreak = 0;
          var totalLates = 0;
          var TotalAbsents = 0;

          results.Add(new AllEmployeeTimesheetDTO(val.EmployeeId, val.Name, val.Department, val.Section, val.Charging, val.Timesheets, totalWorkHours, totalBreak, totalLates, TotalAbsents));
        }
      }
    }

    return Result<PagedResult<AllEmployeeTimesheetDTO>>.Success(new PagedResult<AllEmployeeTimesheetDTO>(results, totalCount));
  }
}
