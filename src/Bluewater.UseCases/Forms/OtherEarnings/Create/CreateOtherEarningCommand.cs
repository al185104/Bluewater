using Ardalis.Result;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.Create;
public record CreateOtherEarningCommand(Guid empId, OtherEarningType type, decimal totalAmount, bool isActive, DateOnly date) : Ardalis.SharedKernel.ICommand<Result<Guid>>;