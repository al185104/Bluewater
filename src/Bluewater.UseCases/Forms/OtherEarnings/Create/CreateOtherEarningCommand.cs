using Ardalis.Result;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Create;
public record CreateOtherEarningCommand(Guid empId, OtherEarningTypeDTO? type, decimal? totalAmount, bool isActive, DateOnly? date) : Ardalis.SharedKernel.ICommand<Result<Guid>>;