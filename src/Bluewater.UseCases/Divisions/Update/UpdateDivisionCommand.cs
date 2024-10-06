using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Divisions.Update;
public record UpdateDivisionCommand(Guid DivisionId, string NewName, string? Description) : ICommand<Result<DivisionDTO>>;
