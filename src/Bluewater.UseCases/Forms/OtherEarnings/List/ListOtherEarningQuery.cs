using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
public record ListOtherEarningQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<OtherEarningDTO>>>;
