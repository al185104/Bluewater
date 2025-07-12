using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Forms.Undertimes.List;
public record ListUndertimeQuery(int? skip, int? take, Tenant tenant) : IQuery<Result<IEnumerable<UndertimeDTO>>>;
