using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.EmployeeTypes.Update;
public record UpdateEmployeeTypeCommand(Guid EmployeeTypeId, string NewName, string Value, bool IsActive) : ICommand<Result<EmployeeTypeDTO>>;
