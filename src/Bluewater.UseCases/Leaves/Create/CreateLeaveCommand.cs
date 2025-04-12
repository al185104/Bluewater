using Ardalis.Result;
using Bluewater.Core.Forms.Enum;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.Create;

public record CreateLeaveCommand(DateTime? startDate, DateTime? endDate, bool isHalfDay, Guid employeeId, Guid leaveCreditId) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
