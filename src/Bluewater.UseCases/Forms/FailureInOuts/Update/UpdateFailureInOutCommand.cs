using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;
namespace Bluewater.UseCases.Forms.FailureInOuts.Update;
public record UpdateFailureInOutCommand(Guid id, Guid? empId, DateTime date, string remarks, FailureInOutReason reason, ApplicationStatus status) : ICommand<Result<FailureInOutDTO>>;
