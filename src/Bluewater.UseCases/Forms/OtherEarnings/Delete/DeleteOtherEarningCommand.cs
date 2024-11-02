using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.OtherEarnings.Delete;
public record DeleteOtherEarningCommand(Guid OtherEarningId) : ICommand<Result>;
