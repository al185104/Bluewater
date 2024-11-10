using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.Update;
public record UpdateDeductionCommand(Guid id, Guid empId, DeductionsTypeDTO type, decimal totalAmount, decimal monthlyAmortization, decimal remainingBalance, int noOfMonths, DateOnly startDate, DateOnly endDate, string remarks, ApplicationStatusDTO status) : ICommand<Result<DeductionDTO>>;
