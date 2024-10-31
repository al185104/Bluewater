using Ardalis.Result;
using Bluewater.Core.Forms.Enum;

namespace Bluewater.UseCases.Deductions.Create;

public record CreateDeductionCommand(Guid empId, DeductionsType type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks) : Ardalis.SharedKernel.ICommand<Result<Guid>>;