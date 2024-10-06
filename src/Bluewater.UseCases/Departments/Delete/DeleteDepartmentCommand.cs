using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Departments.Delete;
public record DeleteDepartmentCommand(Guid DepartmentId) : ICommand<Result>;
