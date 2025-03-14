using Ardalis.Result;

namespace Bluewater.UseCases.Timesheets.Create;

public record CreateTimesheetFromTimeLogCommand(string barcode, DateTime? timeInput, DateOnly? entryDate, int inputType) : Ardalis.SharedKernel.ICommand<Result<string>>;

