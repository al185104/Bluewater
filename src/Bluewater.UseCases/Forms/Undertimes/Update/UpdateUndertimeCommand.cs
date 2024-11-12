using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Undertimes.Update;
public record UpdateUndertimeCommand(Guid id, Guid empId, decimal inclusiveTime, string reason, DateOnly date, ApplicationStatusDTO status) : ICommand<Result<UndertimeDTO>>;
