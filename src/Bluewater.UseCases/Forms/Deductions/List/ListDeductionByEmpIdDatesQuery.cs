using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UserCases.Forms.Enum;

namespace Bluewater.UseCases.Forms.Deductions.List;
public record ListDeductionByEmpIdDatesQuery(int? skip, int? take, Guid? empId, ApplicationStatusDTO? status, DateOnly end) : IQuery<Result<IEnumerable<DeductionDTO>>>;
