using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.UseCases.Timesheets.Get;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Bluewater.UseCases.Timesheets.Update;

namespace Bluewater.UseCases.Timesheets.Create;

public class CreateTimesheetFromTimeLogHandler(IRepository<Employee> _empRepository, IServiceScopeFactory serviceScopeFactory) : ICommandHandler<CreateTimesheetFromTimeLogCommand, Result<string>>
{
  public async Task<Result<string>> Handle(CreateTimesheetFromTimeLogCommand request, CancellationToken cancellationToken)
  {
    TimesheetDTO? timesheetDTO = null;

    var spec = new EmployeeByBarcodeSpec(request.barcode);
    var emp = await _empRepository.FirstOrDefaultAsync(spec, cancellationToken);
    if (emp == null)
    {
      return new Result<string>(string.Empty);
    }

    using (var scope = serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var ret = await mediator.Send(new GetTimesheetByEmpIdAndDateQuery(emp.Id, request.entryDate ?? DateOnly.MinValue));
      if (ret.IsSuccess)
        timesheetDTO = ret.Value;
    }

    var input = request.inputType;
    if (input == 3 && timesheetDTO != null && timesheetDTO.TimeOut1 == null)
      input = 1;

    switch (input)
    {
      case 0:
        {
          using var scope = serviceScopeFactory.CreateScope();
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var timesheet = new CreateTimesheetCommand(employeeId: emp.Id, timeIn1: request.timeInput, timeOut1: null, timeIn2: null, timeOut2: null, entryDate: request.entryDate);
          var result = await mediator.Send(timesheet, cancellationToken);
          if (result.IsSuccess)
            return new Result<string>($"{emp.LastName}, {emp.FirstName}");
          break;
        }
      case 1:
        if (timesheetDTO != null)
        {
          using var scope = serviceScopeFactory.CreateScope();
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          var _result = await mediator.Send(new UpdateSingleTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, request.timeInput, TimesheetEnum.TimeOut1, request.entryDate ?? DateOnly.FromDateTime(DateTime.Today)));
          if (_result.IsSuccess)
            return new Result<string>($"{emp.LastName}, {emp.FirstName}");
        }
        break;
      case 2:
        if (timesheetDTO != null)
        {
          using (var scope = serviceScopeFactory.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var _result = await mediator.Send(new UpdateSingleTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, request.timeInput, TimesheetEnum.TimeIn2, request.entryDate ?? DateOnly.FromDateTime(DateTime.Today)));
            if (_result.IsSuccess)
              return new Result<string>($"{emp.LastName}, {emp.FirstName}");
          }
        }
        break;
      case 3:
        if (timesheetDTO != null)
        {
          using (var scope = serviceScopeFactory.CreateScope())
          {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var _result = await mediator.Send(new UpdateSingleTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, request.timeInput, TimesheetEnum.TimeOut2, request.entryDate ?? DateOnly.FromDateTime(DateTime.Today)));
            if (_result.IsSuccess)
              return new Result<string>($"{emp.LastName}, {emp.FirstName}");
          }
        }
        break;
    }

    return new Result<string>(string.Empty);
  }
}

