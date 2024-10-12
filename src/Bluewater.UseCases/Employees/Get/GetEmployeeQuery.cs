using Ardalis.Result;
using Bluewater.UseCases.Employees;

public record GetEmployeeQuery(Guid EmployeeId) : Ardalis.SharedKernel.IQuery<Result<EmployeeDTO>>;