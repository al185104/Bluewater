using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Sections.Delete;
public record DeleteSectionCommand(Guid SectionId) : ICommand<Result>;
