using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Deductions;
public record DeductionDTO(Guid id, Guid? empId, DeductionsType type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks, ApplicationStatus status);