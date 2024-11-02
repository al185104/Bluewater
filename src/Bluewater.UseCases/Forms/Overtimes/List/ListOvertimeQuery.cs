using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Overtimes.List;
public record ListOvertimeQuery(int? skip, int? take) : IQuery<Result<IEnumerable<OvertimeDTO>>>;
