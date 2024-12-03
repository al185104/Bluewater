using Ardalis.Result;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.Create;
using Bluewater.UseCases.Timesheets.Get;
using Bluewater.UseCases.Timesheets.Update;
using FastEndpoints;
using MediatR;

namespace Bluewater.Web.Timesheets;
/// <summary>
/// Creates a new Timesheet
/// </summary>
/// <param name="_mediator"></param>
/// <param name="serviceScopeFactory"></param>
public class Create(IMediator _mediator, IServiceScopeFactory serviceScopeFactory) : Endpoint<CreateTimesheetRequest, CreateTimesheetsResponse>
{
  public override void Configure()
  {
    Post(CreateTimesheetRequest.Route);
    AllowAnonymous();
    //Summary(s => { s.ExampleRequest = new CreateTimesheetRequest {  }; });
  }

  public override async Task HandleAsync(CreateTimesheetRequest request, CancellationToken cancellationToken)
  {
    Result<Guid> result = new Result<Guid>(Guid.Empty);
    TimesheetDTO? timesheetDTO = null;
    using(var scope = serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var ret = await mediator.Send(new GetTimesheetByEmpIdAndDateQuery(request.employeeId, request.entryDate ?? DateOnly.MinValue));
        if(ret.IsSuccess)
            timesheetDTO = ret.Value;
    }

    switch(request.inputType)
    {
        case 0:
            var timesheet = new CreateTimesheetCommand(employeeId: request.employeeId, timeIn1: request.timeInput, timeOut1: null, timeIn2: null, timeOut2: null, entryDate: request.entryDate);
            result = await _mediator.Send(timesheet, cancellationToken);
            break;
        case 1:
            if(timesheetDTO != null) {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var _result = await mediator.Send(new UpdateTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, 
                        timesheetDTO.TimeIn1, 
                        request.timeInput,
                        timesheetDTO.TimeIn2, 
                        timesheetDTO.TimeOut2, 
                        timesheetDTO.EntryDate, false));
                    if(_result.IsSuccess)
                        result = _result.Value.Id;
                }
            }
            break;
        case 2:
            if(timesheetDTO != null) {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var _result = await mediator.Send(new UpdateTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, 
                        timesheetDTO.TimeIn1, 
                        timesheetDTO.TimeOut1,
                        request.timeInput,
                        timesheetDTO.TimeOut2, 
                        timesheetDTO.EntryDate, false));
                    if(_result.IsSuccess)
                        result = _result.Value.Id;
                }
            }
            break;
        case 3:
            if(timesheetDTO != null) {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var _result = await mediator.Send(new UpdateTimesheetCommand(timesheetDTO.Id, timesheetDTO.EmployeeId, 
                        timesheetDTO.TimeIn1, 
                        timesheetDTO.TimeOut1,
                        timesheetDTO.TimeIn2, 
                        request.timeInput,
                        timesheetDTO.EntryDate, false));
                    if(_result.IsSuccess)
                        result = _result.Value.Id;
                }
            }
            break;
        default:
            result = new Result<Guid>(Guid.Empty);
            break;
    }

    Response = new CreateTimesheetsResponse(result.Value);
    return;
  }
}
