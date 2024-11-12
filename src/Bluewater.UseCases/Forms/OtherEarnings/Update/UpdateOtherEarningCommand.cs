using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Update;
public record UpdateOtherEarningCommand(Guid id, Guid empId, OtherEarningTypeDTO type, decimal totalAmount, bool isActive, DateOnly date, ApplicationStatusDTO status) : ICommand<Result<OtherEarningDTO>>;
