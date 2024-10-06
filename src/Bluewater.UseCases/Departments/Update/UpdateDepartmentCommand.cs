using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Departments.Update;
public record UpdateDepartmentCommand(Guid DepartmentId, string NewName, string? Description, Guid divisionId) : ICommand<Result<DepartmentDTO>>;
