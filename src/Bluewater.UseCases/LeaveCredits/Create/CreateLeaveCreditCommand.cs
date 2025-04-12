using Ardalis.Result;

namespace Bluewater.UseCases.LeaveCredits.Create;

public record CreateLeaveCreditCommand(string Code, string? Description, decimal? Credit, int? SortOrder, bool IsLeaveWithPay, bool IsCanCarryOver) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
