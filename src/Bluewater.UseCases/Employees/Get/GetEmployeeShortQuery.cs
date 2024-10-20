using Ardalis.Result;
using Bluewater.UseCases.Employees;

public record GetEmployeeShortQuery(string EmployeeName) : Ardalis.SharedKernel.IQuery<Result<EmployeeShortDTO>>;