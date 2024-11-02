using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Update;
public record UpdateOtherEarningCommand(Guid id, Guid empId, OtherEarningType type, decimal totalAmount, bool isActive, DateOnly date, ApplicationStatus status) : ICommand<Result<OtherEarningDTO>>;
