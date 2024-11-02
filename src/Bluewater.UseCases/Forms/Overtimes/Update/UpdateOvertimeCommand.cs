using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Forms.Overtimes.Update;
public record UpdateOvertimeCommand(Guid id, Guid empId, DateTime startDate, DateTime endDate, int approvedHours, string remarks, ApplicationStatus status) : ICommand<Result<OvertimeDTO>>;
