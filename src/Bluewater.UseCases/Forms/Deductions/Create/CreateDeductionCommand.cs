using Ardalis.Result;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.Create;

public record CreateDeductionCommand(Guid empId, DeductionsTypeDTO? type, decimal? totalAmount, decimal? monthlyAmortization, decimal? remainingBalance, int? noOfMonths, DateOnly? startDate, DateOnly? endDate, string? remarks) : Ardalis.SharedKernel.ICommand<Result<Guid>>;