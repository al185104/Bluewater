using Ardalis.Result;

namespace Bluewater.UseCases.Timesheets.Create;
public record CreateTimesheetCommand(Guid employeeId, DateTime? timeIn1, DateTime? timeOut1, DateTime? timeIn2, DateTime? timeOut2, DateOnly entryDate) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
