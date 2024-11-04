using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Overtimes.List;
public record ListOvertimeByDatesQuery(int? skip, int? take, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<OvertimeDTO>>>;
