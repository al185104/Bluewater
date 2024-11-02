using Ardalis.Result;
namespace Bluewater.UseCases.Forms.Overtimes.Create;
public record CreateOvertimeCommand(Guid empId, DateTime? startDate, DateTime? endDate, int? approvedHours, string? remarks) : Ardalis.SharedKernel.ICommand<Result<Guid>>;