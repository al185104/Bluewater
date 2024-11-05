using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
public record ListOtherEarningsByEmpIdDatesQuery(int? skip, int? take, Guid empId, DateOnly start, DateOnly end) : IQuery<Result<IEnumerable<OtherEarningDTO>>>;
