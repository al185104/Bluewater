using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Deductions.Update;
public record UpdateDeductionCommand(Guid id, Guid empId, DeductionsType type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks, ApplicationStatus status) : ICommand<Result<DeductionDTO>>;
