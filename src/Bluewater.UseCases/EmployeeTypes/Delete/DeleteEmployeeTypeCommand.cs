using Ardalis.Result;
namespace Bluewater.UseCases.EmployeeTypes.Delete;
public record DeleteEmployeeTypeCommand(Guid EmployeeTypeId) : Ardalis.SharedKernel.ICommand<Result>;