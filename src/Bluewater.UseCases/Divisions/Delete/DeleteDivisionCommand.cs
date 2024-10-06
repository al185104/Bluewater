using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Divisions.Delete;
public record DeleteDivisionCommand(Guid DivisionId) : ICommand<Result>;
