using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;
namespace Bluewater.UseCases.Forms.FailureInOuts.Update;
public record UpdateFailureInOutCommand(Guid id, Guid? empId, DateTime date, string remarks, FailureInOutReasonDTO reason, ApplicationStatusDTO status) : ICommand<Result<FailureInOutDTO>>;
