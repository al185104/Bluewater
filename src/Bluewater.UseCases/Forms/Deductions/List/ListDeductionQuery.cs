using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Forms.Deductions.List;
public record ListDeductionQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<DeductionDTO>>>;
