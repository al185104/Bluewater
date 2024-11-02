using Bluewater.Core.Forms.Enum;
namespace Bluewater.UseCases.Forms.OtherEarnings;
public record OtherEarningDTO(Guid id, Guid? empId, decimal totalAmount, bool isActive, DateOnly date, ApplicationStatus status);