using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Leaves.Update;
public record UpdateLeaveCommand(Guid LeaveId, DateTime startDate, DateTime endDate, bool isHalfDay, ApplicationStatusDTO status, Guid employeeId, Guid leaveCreditId) : ICommand<Result<LeaveDTO>>;
