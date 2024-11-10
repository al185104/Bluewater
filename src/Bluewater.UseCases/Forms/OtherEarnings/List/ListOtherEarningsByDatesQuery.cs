
using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
public record ListOtherEarningsByDatesQuery(int? skip, int? take, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<OtherEarningDTO>>>;
