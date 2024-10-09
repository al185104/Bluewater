using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Employees.Delete;
public record DeleteEmployeeCommand(Guid EmployeeId) : ICommand<Result>;
