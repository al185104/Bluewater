using Ardalis.Result;
namespace Bluewater.UseCases.EmployeeTypes.Create
{
    public record CreateEmployeeTypeCommand(string Name, string Value, bool IsActive) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
}