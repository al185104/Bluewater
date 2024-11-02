using Ardalis.Result;
using Bluewater.UserCases.Forms.Enum;


namespace Bluewater.UseCases.Forms.FailureInOuts.Create;
public record CreateFailureInOutCommand(Guid empId, DateTime? date, string? remarks, FailureInOutReasonDTO? reason) : Ardalis.SharedKernel.ICommand<Result<Guid>>;