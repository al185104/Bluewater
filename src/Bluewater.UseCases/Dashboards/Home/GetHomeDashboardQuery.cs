using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate.Enum;

namespace Bluewater.UseCases.Dashboards.Home;

public record GetHomeDashboardQuery(Tenant Tenant) : IQuery<Result<HomeDashboardDTO>>;
