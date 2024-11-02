using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Overtimes.Get;
public record GetOvertimeQuery(Guid OvertimeId) : IQuery<Result<OvertimeDTO>>;
